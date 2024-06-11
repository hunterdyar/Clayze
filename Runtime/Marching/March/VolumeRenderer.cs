using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace Clayze.Marching
{
	/// <summary>
	/// A Volume Renderer Instantiates a number of VolumeChunks, according to Division. These chunks get the mesh renderer, and update themselves with their own pointcache's and compute shader dispatches.
	/// The Renderer listens for volume updates, and tells the appropriate chunks to update using unrotated bounding boxes.
	/// The number of chunk upates per frame will affect performance. Chunks are sorted so that the ones closest to the camera update first.
	/// Future optimizations....
	///   - having chunks know if they are full or empty. If so, they can skip processing. If they are full, we can start occlusion culling or de-prioritizing updates.
	///   - Frustum culling for chunk updates. Or even a naieve 'is behind the camera' plane culling: closest to camera sort does not know direction.
	///   - 
	/// </summary>
	[RequireComponent(typeof(Volume))]
	public class VolumeRenderer : MonoBehaviour
	{
		public Volume Volume => _volume;
		private Volume _volume;
		[SerializeField] private Transform _meshParent;
		[Min(1)][Tooltip("Chunks per axis.")]
		[SerializeField] private int _divisions;//not number of slices, but number of chunks per axis.
		[SerializeField] private Material _material;
		[Min(1)]
		[SerializeField] private int chunkUpdatesPerFrame;
		
		private int _chunkSize;//points per chunk in volume space.

		private readonly Dictionary<Vector3Int, VolumeChunk> _chunks = new Dictionary<Vector3Int, VolumeChunk>();
		[Header("Pass-Through Configuration")] public ComputeShader MarchingCompute;
		[Range(0, 1)] public float smoothness;
		public float SurfaceLevel = 0;

		private readonly List<VolumeChunk> _chunkNeedingUpdate = new List<VolumeChunk>();
		//Debugging
		private Vector3Int _lastEditMin;
		private Vector3Int _lastEditMax;
		private void Awake()
		{
			_volume = GetComponent<Volume>();
			GenerateChunks();
		}

		private void OnEnable()
		{
			_volume.OnChange += VolumeChange;
		}
		
		private void OnDisable()
		{
			_volume.OnChange -= VolumeChange;
		}

		public VolumeChunk GetChunkForWorldPoint(Vector3 worldPos, out Vector3Int local)
		{
			local = _volume.WorldToLocal(worldPos);
			var key = new Vector3Int(local.x / _chunkSize, local.y / _chunkSize, local.z / _chunkSize);
			if (_chunks.TryGetValue(key, out var chunk))
			{
				return chunk;
			}
			
			return null;
		}

		public bool TryGetPoint(Vector3 worldPos, out float point)
		{
			//todo: this needs to do a bilinear interpolation the results...
			//on the marching compute shader, we use this for the "smoothness" (which is broken?)
						//float3 InterpolateVertices(float4 v1, float4 v2) {
					// 	float s = (surfaceLevel - v1.w) / (v2.w - v1.w);
					// 	float t = (1 - smoothness) * 0.5 + s * smoothness;
					// 	return v1.xyz + t * (v2.xyz - v1.xyz); }
			var c = GetChunkForWorldPoint(worldPos, out var local);
			if (c != null)
			{
				point = c.GetPoint(local);
				return true;
			}

			point = 0;
			return false;
		}
		private void VolumeChange(Vector3Int boundsMin, Vector3Int boundsMax)
		{
			if(!GeometryUtility.CubesIntersect(boundsMin, boundsMax, Vector3Int.zero, new Vector3Int(_volume.Size, _volume.Size, _volume.Size)))
			{
				return;
			}
			//debugging, used to draw gizmos.
			_lastEditMin = boundsMin;
			_lastEditMax = boundsMax;

			//for every chunk overlapping the bounds of what changed, update it.
			foreach (var kvp in _chunks)
			{
				var c = kvp.Value;
				
				if (GeometryUtility.CubesIntersect(c.PointsMin, c.PointsMax, boundsMin, boundsMax))
				{
					var key = _volume.IndexFromCoord(kvp.Key.x, kvp.Key.y, kvp.Key.z);
					c.DebugGizmoColor = Color.blue;
					if (!_chunkNeedingUpdate.Contains(kvp.Value))
					{
						_chunkNeedingUpdate.Add(kvp.Value);
						
						//debugging
						
						c.DebugGizmoColor = Color.Lerp(Color.green,Color.magenta, 1-(_chunkNeedingUpdate.IndexOf(kvp.Value)/(float)_chunkNeedingUpdate.Count));
					}
				}
				else
				{
					c.DebugGizmoColor = Color.white;
				}
			}

			//todo: We don't need to sort this every frame, only when the camera moves, and even then, only after it moves a certain amount. Problem is, we keep adding/removing from this.
			//so as is, it won't "stay sorted".
			
			//Whats faster, keeping a dictionary? "updateIfNeeded" and return true/false with a count, in our always-sorted list?
			//how slow is this function?
			_chunkNeedingUpdate.Sort(SortChunkByDistance);
		}

		public int SortChunkByDistance(VolumeChunk a, VolumeChunk b)
		{
			var ad = GeometryUtility.DistanceFromCamera(a.WorldCenter);
			var bd = GeometryUtility.DistanceFromCamera(b.WorldCenter);
			return ad.CompareTo(bd);
		}

		private void Update()
		{
			//todo: we would like to keep the chunks in a sorted list, and only update the ones closest to camera.
			//todo: we would like to isolated-update the chunks that are out of the camera frustum, on the interior of meshes, empty, or otherwise irrelevant. we can't do this chunk-wise (chunks might be empty). 
			
			int count = _chunkNeedingUpdate.Count;
			if (count > 0)
			{
				int updateThisFrame = Mathf.Min(chunkUpdatesPerFrame, _chunkNeedingUpdate.Count); 
				for (int i = 0; i < updateThisFrame; i++)
				{
					var c = _chunkNeedingUpdate[0];
					c.UpdateMesh(true);//todo: from settings
					_chunkNeedingUpdate.RemoveAt(0);
				}
			}
		}

		public void GenerateChunks()
		{
			//todo: runtime reset, destroy children.
			
			_chunks.Clear();
			if (_volume.Size % _divisions != 0)
			{
				Debug.LogWarning("Bad Chunk Size. Must be even division of size.");
			}
			// ReSharper disable once PossibleLossOfFraction
			_chunkSize = Mathf.CeilToInt(_volume.Size / _divisions);//we should just divide and check if it's a perfect division or not.
			for (int i = 0; i < _divisions; i++)
			{
				for (int j = 0; j < _divisions; j++)
				{
					for (int k = 0; k < _divisions; k++)
					{
						Vector3Int chunkPos = new Vector3Int(i, j, k);

						GameObject chunk = new GameObject();
						chunk.transform.SetParent(_meshParent);
						chunk.name = $"Chunk - {i},{j},{k}";
						chunk.AddComponent<MeshFilter>();
						var mr = chunk.AddComponent<MeshRenderer>();
						mr.material = _material;
						var gen = chunk.AddComponent<VolumeChunk>();

						//Set appropriate points bounds.
						gen.Coord = new Vector3Int(i, j, k);
						var min = gen.Coord * _chunkSize;
						var max = new Vector3Int(min.x + _chunkSize, min.y + _chunkSize, min.z + _chunkSize)+Vector3Int.one;
						gen.Initialize(this, _volume, min, max);

						_chunks.Add(chunkPos,gen);
					}
				}
			}
		}

		private void OnDrawGizmos()
		{
			if (_volume == null)
			{
				_volume = GetComponent<Volume>();
			}
			Gizmos.color = Color.red;
			Vector3 a = _volume.VolumeToWorld(_lastEditMin);
			Vector3 b = _volume.VolumeToWorld(_lastEditMax);
			
			Gizmos.DrawWireCube((a + b) / 2, b - a);
		}
	}
}