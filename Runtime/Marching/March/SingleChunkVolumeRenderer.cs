using System.Collections.Generic;
using Clayze;
using Clayze.Marching;
using UnityEngine;
using GeometryUtility = UnityEngine.GeometryUtility;

namespace Marching.March
{
	[RequireComponent(typeof(Volume))]
	public class SingleChunkVolumeRenderer : MonoBehaviour, IVolumeRenderer
	{
		public Volume Volume => _volume;
		private Volume _volume;
		[SerializeField] private Transform _meshParent;
		[SerializeField] private Material _material;

		private VolumeChunk _chunk;
		public ComputeShader MarchingCompute => _marchingCompute;

		[Header("Pass-Through Configuration")] [SerializeField]
		private ComputeShader _marchingCompute;

		public float Smoothness => _smoothness;
		[Range(0, 1), SerializeField] private float _smoothness;
		public float SurfaceLevel => _surfaceLevel;
		[SerializeField] private float _surfaceLevel = 0.01f;

		private bool _needsUpdate = false;
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
		private void VolumeChange(Vector3Int boundsMin, Vector3Int boundsMax)
		{
			if(!Clayze.Marching.GeometryUtility.CubesIntersect(boundsMin, boundsMax, Vector3Int.zero, new Vector3Int(_volume.Size, _volume.Size, _volume.Size)))
			{
				return;
			}
			//debugging, used to draw gizmos.
			_lastEditMin = boundsMin;
			_lastEditMax = boundsMax;
			_needsUpdate = true;
		}
		
		public int SortChunkByDistance(VolumeChunk a, VolumeChunk b)
		{
			var ad = Clayze.Marching.GeometryUtility.DistanceFromCamera(a.WorldCenter);
			var bd = Clayze.Marching.GeometryUtility.DistanceFromCamera(b.WorldCenter);
			return ad.CompareTo(bd);
		}

		private void Update()
		{
			//todo: we would like to keep the chunks in a sorted list, and only update the ones closest to camera.
			//todo: we would like to isolated-update the chunks that are out of the camera frustum, on the interior of meshes, empty, or otherwise irrelevant. we can't do this chunk-wise (chunks might be empty). 
			
			if (_needsUpdate)
			{
				_chunk.UpdateMesh(true);//todo: from settings
				_needsUpdate = false;
			}
		}

		public void GenerateChunks()
		{
			Vector3Int chunkPos = new Vector3Int(0, 0, 0);

			GameObject chunk = new GameObject();
			chunk.transform.SetParent(_meshParent);
			chunk.name = $"Chunk";
			chunk.AddComponent<MeshFilter>();
			var mr = chunk.AddComponent<MeshRenderer>();
			mr.material = _material;
			var gen = chunk.AddComponent<VolumeChunk>();

			//Set appropriate points bounds.
			gen.Coord = new Vector3Int(0, 0, 0);
			var min = gen.Coord;
			var max = new Vector3Int(_volume.Size, _volume.Size,_volume.Size)+Vector3Int.one;
			gen.Initialize(this, _volume, min, max);
			
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