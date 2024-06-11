using System;
using System.Collections;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using Object = System.Object;

namespace Clayze.Marching
{
	public class VolumeChunk : MonoBehaviour
	{
		//setup
		public bool ConstantRefresh;
		private Volume _volume;
		private VolumeRenderer _volumeRenderer;

		private ComputeShader MarchingCubeCompute => _volumeRenderer.MarchingCompute;
		private float SurfaceLevel => _volumeRenderer.SurfaceLevel;
		private float Smoothness => _volumeRenderer.smoothness;
		
		public Vector3Int PointsMin { get; private set; }
		public Vector3Int PointsMax { get; private set; }
		private int Size;
		
		public Vector3Int Coord { get; set; }

		[HideInInspector]
		public Color DebugGizmoColor = Color.white;

		private MeshRenderer _meshRenderer;
		private MeshFilter _meshFilter;
		
		//settings
		private int _threadsPerAxis;
		private bool updatedThisFrame;

		public Vector3 WorldCenter => _worldCenter;
		private Vector3 _worldCenter;
		
		//Properties
		ComputeBuffer _pointsBuffer;
		GraphicsBuffer _triangleBuffer;
		GraphicsBuffer _triangleCountBuffer;
		private CommandBuffer _computeCommandBuffer;
		private Triangle[] _tris;
		private Mesh _mesh;
		private bool _tryAsync;

		//points cache for physics/sample operationns
		private NativeArray<float> _pointsCache = new NativeArray<float>();
		private Action<AsyncGPUReadbackRequest> RequestUpdatePointsCacheCallback;
		private bool _pointsCacheIsDirty;
		
		//shader prop cache
		private static readonly int PointsPropName = Shader.PropertyToID("points");
		private static readonly int TrianglesPropName = Shader.PropertyToID("triangles");
		private static readonly int PointsPerAxisPropName = Shader.PropertyToID("pointsPerAxis");
		private static readonly int LevelPropName = Shader.PropertyToID("surfaceLevel");
		private static readonly int SmoothnessPropName = Shader.PropertyToID("smoothness");
		
		private void Awake()
		{
			_meshFilter = GetComponent<MeshFilter>();
			_meshRenderer = GetComponent<MeshRenderer>();
			
			//create a new mesh to do out thing
			_mesh = new Mesh
			{
				indexFormat = IndexFormat.UInt32
			};
			_meshFilter.sharedMesh = _mesh;
			
			//Called by physics operations and such automatically.
			RequestUpdatePointsCacheCallback += r =>
			{
				_pointsCacheIsDirty = false;
			};
		}

		public void Initialize(VolumeRenderer volumeRenderer, Volume volume, Vector3Int min, Vector3Int max)
		{
			_volumeRenderer = volumeRenderer;
			_volume = volume;
			PointsMin = min;
			PointsMax = max;
			Size = max.x-min.x;
			
			CreateBuffers();
			ConfigureThreadsPerAxis();
			_worldCenter = _volume.transform.TransformPoint((_volume.VolumeToWorld(PointsMax - Vector3Int.one) +
			                                                 _volume.VolumeToWorld(PointsMin)) / 2);
			UpdateMesh(false);//todo: this and in start? uh oh

		}

		private void Start()
		{
			CreateBuffers();
			ConfigureThreadsPerAxis();
			UpdateMesh(false);//first frame, no async we can to freeze and load.
		}

		private void Update()
		{
			if (ConstantRefresh)
			{
				UpdateMesh(_tryAsync);
			}
		}

		void CreateBuffers()
		{
			int numPoints = _volume.TotalPointCount;
			int voxelsPerAxis = _volume.Size - 1;//points are volume determined
			int numVoxels = voxelsPerAxis * voxelsPerAxis * voxelsPerAxis;
			int maxTriangleCount = numVoxels * 5;//5 is max triangles per cube march shape... I believe.
			
			if (_pointsBuffer == null || numPoints != _pointsBuffer.count)
			{
				ReleaseBuffers();
				_triangleBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Append,maxTriangleCount, sizeof(float) * 3 * 3);
				_pointsBuffer = new ComputeBuffer(numPoints, sizeof(float) * 4,ComputeBufferType.Append);
				_triangleCountBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Raw,1, sizeof(int));
				_computeCommandBuffer = new CommandBuffer();
				if (SystemInfo.supportsAsyncGPUReadback)
				{
					_computeCommandBuffer.SetExecutionFlags(CommandBufferExecutionFlags.AsyncCompute);
				}
			}

			_pointsCache = new NativeArray<float>(numPoints*4, Allocator.Persistent);
		}

		private void UpdatePointsCache()
		{
			if (_pointsBuffer == null)
			{
				return;
			}
			if (_pointsCacheIsDirty)
			{
				AsyncGPUReadback.RequestIntoNativeArray(ref _pointsCache, _pointsBuffer, RequestUpdatePointsCacheCallback);
			}
		}

		public float GetPoint(Vector3Int volumePos)
		{
			UpdatePointsCache();
			int x = volumePos.x;
			int y = volumePos.y;
			int z = volumePos.z;
			int id = z * Size * Size + y * Size + x;
			if (id >= 0 && id < _pointsCache.Length)
			{
				return _pointsCache[id];
			}
			else
			{
				Debug.LogWarning("Getting point from chunk that is out of bounds.");
				return 0;
			}
			//get the position of the v3 between local min and local max...
			//convert to 1d with size...
			//get from cache
			//Size
		}

		private void ConfigureThreadsPerAxis()
		{
			int voxelsPerAxis = Size - 1; //hello, fence post
			int threadGroupSize = 1;
			_threadsPerAxis = Mathf.CeilToInt(voxelsPerAxis / (float)threadGroupSize);
			//it breaks when numThreadsPerAxis is 1,1,1. So we're just gonna set the min to 2.
			_threadsPerAxis = _threadsPerAxis == 1 ? 2 : _threadsPerAxis;
			//float surfaceLevel = SurfaceLevel;
		}
		
		[ContextMenu("Update Mesh")]
		public void UpdateMesh(bool asyncReadback)
		{
			_pointsCacheIsDirty = true;
            //Update the points buffer from the volume. Processes and copies data from CPU to GPU.
            _volume.GenerateInBounds(ref _pointsBuffer,PointsMin,PointsMax);
            
            _triangleBuffer.SetCounterValue(0);//computer buffer uses appends
            MarchingCubeCompute.SetBuffer(0, PointsPropName, _pointsBuffer);
            MarchingCubeCompute.SetBuffer(0, TrianglesPropName, _triangleBuffer);
            MarchingCubeCompute.SetInt(PointsPerAxisPropName, Size);//volume.size
            MarchingCubeCompute.SetFloat(LevelPropName, SurfaceLevel);
            MarchingCubeCompute.SetFloat(SmoothnessPropName,Smoothness);
            //do the thing.
            if (!asyncReadback || !SystemInfo.supportsAsyncGPUReadback)
            {
	            MarchingCubeCompute.Dispatch(0, _threadsPerAxis, _threadsPerAxis, _threadsPerAxis);
	            StartCoroutine(CreateMesh(false));
            }
            else
            {
	            _computeCommandBuffer.DispatchCompute(MarchingCubeCompute,0, _threadsPerAxis, _threadsPerAxis,
		            _threadsPerAxis);
	            Graphics.ExecuteCommandBufferAsync(_computeCommandBuffer,ComputeQueueType.Default);
	            //AsyncGPUReadback.Request(MarchingCubeCompute, CreateMesh);
	            //when that's done...
	            StartCoroutine(CreateMesh(true));
            }
		}

		private IEnumerator CreateMesh(bool asyncReadback)
		{
			//todo: use a graphics fence to stop the gpu to wait for the cpu?
			
			// Get number of triangles in the triangle buffer
			GraphicsBuffer.CopyCount(_triangleBuffer, _triangleCountBuffer, 0);
			int[] triCountArray = { 0 }; //the buffer is just an array of a thing
			_triangleCountBuffer.GetData(triCountArray);
			int numTris = triCountArray[0];

			// Get triangle data from shader

			var ntris = new NativeArray<Triangle>();
			if (asyncReadback)
			{
				var request = AsyncGPUReadback.Request(_triangleBuffer);
				while (!request.done)
				{
					yield return null;
				}
				if (request.hasError == false)
				{
					ntris = request.GetData<Triangle>(0);
				}
				else
				{
					Debug.Log("Something has gone wrong with an async readback request. Abandoning efforts.");
					yield break;
				}
			}
			else
			{
				//reduce garbage collection by only growing the triangle array.
				//so we have to remember to use numTris instead of _tris.length when we iterate.
				if (_tris == null || numTris > _tris.Length)
				{
					_tris = new Triangle[numTris];
				}

				_triangleBuffer.GetData(_tris, 0, 0, numTris);
			}
			
			//cpu side mesh creation
			_mesh.Clear();

			var vertices = new Vector3[numTris * 3];
			var meshTriangles = new int[numTris * 3];

			if (asyncReadback)
			{
				for (int i = 0; i < numTris; i++)
				{
					for (int j = 0; j < 3; j++)
					{
						meshTriangles[i * 3 + j] = i * 3 + j;
						vertices[i * 3 + j] = ntris[i][j]; //times 2?
					}
				}
				
			}
			else
			{
				for (int i = 0; i < numTris; i++)
				{
					for (int j = 0; j < 3; j++)
					{
						meshTriangles[i * 3 + j] = i * 3 + j;
						// ReSharper disable once PossibleNullReferenceException
						vertices[i * 3 + j] = _tris[i][j]; //times 2?
					}
				}
			}
			

			_mesh.vertices = vertices;
			_mesh.triangles = meshTriangles;

			_mesh.RecalculateNormals();
			// _mesh.RecalculateBounds();
			yield break;
		}

		void ReleaseBuffers()
		{
			if (_triangleBuffer != null)
			{
				_triangleBuffer.Release();
			}

			if (_pointsBuffer != null)
			{
				_pointsBuffer.Release();
			}

			if (_triangleCountBuffer != null)
			{
				_triangleCountBuffer.Release();
			}

			if (_computeCommandBuffer != null)
			{
				_computeCommandBuffer.Release();
			}

			_pointsCache.Dispose();
		}

		public ComputeBuffer PointsBuffer()
		{
			return _pointsBuffer;
		}

		private void OnDestroy()
		{
			ReleaseBuffers();
		}

		private void OnDrawGizmos()
		{
			//We secretly draw them too small, because they share boundries with their neighbors, the overlapping is a visual mess in the inspector to look at.
			//for debugging, the color change is accurate, that's determined elsewhere.
			
			//offset interior meshes...
			float size = (Size-1) / _volume.pointsPerUnit;
			Gizmos.color = new Color(DebugGizmoColor.r, DebugGizmoColor.g, DebugGizmoColor.b, 0.2f);
			Gizmos.DrawWireCube(WorldCenter, new Vector3(size, size, size));
		}
	}
}