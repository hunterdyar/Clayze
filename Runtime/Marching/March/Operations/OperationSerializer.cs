﻿using System;
using UnityEngine;

namespace Marching.Operations
{
	public static class OperationSerializer
	{
		//byte 0 is the operation name
		//byte 1 is the operation type.
		//the rest is unique to each operation.
		
		public static byte[] ToBytes(this IOperation op)
		{
			switch (op.OpName)
			{
				case OperationName.Sphere: 
					return ((SphereOp)op).ToBytes();
				case OperationName.Line:
					return ((LineOp)op).ToBytes();
					
			}
			return Array.Empty<byte>();
		}

		#region OperationsToBytesOverloads

		public static byte[] ToBytes(this SphereOp op)
		{
			//2 byte headedr (1), 4 floats(4) = 15
			byte[] data = new byte[2 + 4 * 4];
			data[0] = (byte)OperationName.Sphere;
			data[1] = (byte)op.OperationType;
			//todo: utility function for serializing vector3s
			var posx = BitConverter.GetBytes(op.Center.x);
			posx.CopyTo(data, 2);
			var posy = BitConverter.GetBytes(op.Center.y);
			posy.CopyTo(data, 6);
			var posz = BitConverter.GetBytes(op.Center.z);
			posz.CopyTo(data, 10);
			var floata = BitConverter.GetBytes(op.Radius);
			floata.CopyTo(data, 14);
			return data;
		}

		public static byte[] ToBytes(this LineOp op)
		{
			byte[] data = new byte[2 + 4 * 7];
			data[0] = (byte)OperationName.Sphere;
			data[1] = (byte)op.OperationType;
			//todo: utility function for serializing vector3s
			BitConverter.GetBytes(op.PointA.x).CopyTo(data, 2);
			BitConverter.GetBytes(op.PointA.y).CopyTo(data, 6);
			BitConverter.GetBytes(op.PointA.z).CopyTo(data, 10);
			BitConverter.GetBytes(op.PointA.x).CopyTo(data, 14);
			BitConverter.GetBytes(op.PointA.y).CopyTo(data, 18);
			BitConverter.GetBytes(op.PointA.z).CopyTo(data, 22);
			BitConverter.GetBytes(op.Radius).CopyTo(data, 26);
			return data;
		}

		#endregion
		public static IOperation FromBytes(byte[] data, int start, out int bytesConsumed)
		{
			bytesConsumed = 0;
			if (data.Length == 0)
			{
				return null;
			}
			//[servermessagetype:1][opUniqueID:4][opName:1][OpType:1][ specific to opName ]
			
			var opID = (UInt32)BitConverter.ToUInt32(data, start);
			//offset is array coordinate.
			int offset = start + 4;
			OperationName op = (OperationName)data[offset];
			offset++;
			var opType = (OperationType)data[offset];
			offset++;
			switch (op)
			{
				case OperationName.Pass:
					return null;
				case OperationName.Sphere:
					//[...][CenterX:4][CenterY:4][CenterZ:4][Radius:4]
					var posX = BitConverter.ToSingle(new ArraySegment<byte>(data, offset, 4));
					offset += 4;
					var posY = BitConverter.ToSingle(new ArraySegment<byte>(data, offset, 4));
					offset += 4;
					var posZ = BitConverter.ToSingle(new ArraySegment<byte>(data, offset, 4));
					offset += 4;
					var floatA = BitConverter.ToSingle(new ArraySegment<byte>(data, offset, 4));
					offset += 4;
					var sop= new SphereOp(new Vector3(posX, posY, posZ), floatA, opType);
					sop.UniqueID = opID;
					bytesConsumed = offset - start;
					return sop;
				case OperationName.Line:
					var posAX = BitConverter.ToSingle(new ArraySegment<byte>(data, offset, 4));
					offset += 4;
					var posAY = BitConverter.ToSingle(new ArraySegment<byte>(data, offset, 4));
					offset += 4;
					var posAZ = BitConverter.ToSingle(new ArraySegment<byte>(data, offset, 4));
					offset += 4;
					var posBX = BitConverter.ToSingle(new ArraySegment<byte>(data, offset, 4));
					offset += 4;
					var posBY = BitConverter.ToSingle(new ArraySegment<byte>(data, offset, 4));
					offset += 4;
					var posBZ = BitConverter.ToSingle(new ArraySegment<byte>(data, offset, 4));
					offset += 4;
					var radius = BitConverter.ToSingle(new ArraySegment<byte>(data, offset, 4));
					offset += 4;
					var lop = new LineOp(new Vector3(posAX, posAY, posAZ),new Vector3(posBX,posBY,posBZ), radius, opType);
					lop.UniqueID = opID;
					bytesConsumed = offset - start;
					return lop;
			}

			return null;
		}
	}
}