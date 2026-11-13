using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using Google.ProtocolBuffers;
using Google.ProtocolBuffers.Collections;
using proto.Proto;

namespace proto;

[DebuggerNonUserCode]
[CompilerGenerated]
[GeneratedCode("ProtoGen", "2.4.1.473")]
public sealed class PhoneEvent : GeneratedMessageLite<PhoneEvent, PhoneEvent.Builder>
{
	[DebuggerNonUserCode]
	[CompilerGenerated]
	[GeneratedCode("ProtoGen", "2.4.1.473")]
	public static class Types
	{
		[CompilerGenerated]
		[GeneratedCode("ProtoGen", "2.4.1.473")]
		public enum Type
		{
			MOTION = 1,
			GYROSCOPE,
			ACCELEROMETER,
			DEPTH_MAP,
			ORIENTATION,
			KEY
		}

		[DebuggerNonUserCode]
		[CompilerGenerated]
		[GeneratedCode("ProtoGen", "2.4.1.473")]
		public sealed class MotionEvent : GeneratedMessageLite<MotionEvent, MotionEvent.Builder>
		{
			[DebuggerNonUserCode]
			[CompilerGenerated]
			[GeneratedCode("ProtoGen", "2.4.1.473")]
			public static class Types
			{
				[DebuggerNonUserCode]
				[CompilerGenerated]
				[GeneratedCode("ProtoGen", "2.4.1.473")]
				public sealed class Pointer : GeneratedMessageLite<Pointer, Pointer.Builder>
				{
					[DebuggerNonUserCode]
					[CompilerGenerated]
					[GeneratedCode("ProtoGen", "2.4.1.473")]
					public sealed class Builder : GeneratedBuilderLite<Pointer, Builder>
					{
						private bool resultIsReadOnly;

						private Pointer result;

						protected override Builder ThisBuilder => this;

						public override bool IsInitialized => result.IsInitialized;

						protected override Pointer MessageBeingBuilt => PrepareBuilder();

						public override Pointer DefaultInstanceForType => DefaultInstance;

						public bool HasId => result.hasId;

						public int Id
						{
							get
							{
								return result.Id;
							}
							set
							{
								SetId(value);
							}
						}

						public bool HasNormalizedX => result.hasNormalizedX;

						public float NormalizedX
						{
							get
							{
								return result.NormalizedX;
							}
							set
							{
								SetNormalizedX(value);
							}
						}

						public bool HasNormalizedY => result.hasNormalizedY;

						public float NormalizedY
						{
							get
							{
								return result.NormalizedY;
							}
							set
							{
								SetNormalizedY(value);
							}
						}

						public Builder()
						{
							result = DefaultInstance;
							resultIsReadOnly = true;
						}

						internal Builder(Pointer cloneFrom)
						{
							result = cloneFrom;
							resultIsReadOnly = true;
						}

						private Pointer PrepareBuilder()
						{
							if (resultIsReadOnly)
							{
								Pointer other = result;
								result = new Pointer();
								resultIsReadOnly = false;
								MergeFrom(other);
							}
							return result;
						}

						public override Builder Clear()
						{
							result = DefaultInstance;
							resultIsReadOnly = true;
							return this;
						}

						public override Builder Clone()
						{
							if (resultIsReadOnly)
							{
								return new Builder(result);
							}
							return new Builder().MergeFrom(result);
						}

						public override Pointer BuildPartial()
						{
							if (resultIsReadOnly)
							{
								return result;
							}
							resultIsReadOnly = true;
							return result.MakeReadOnly();
						}

						public override Builder MergeFrom(IMessageLite other)
						{
							if (other is Pointer)
							{
								return MergeFrom((Pointer)other);
							}
							base.MergeFrom(other);
							return this;
						}

						public override Builder MergeFrom(Pointer other)
						{
							if (other == DefaultInstance)
							{
								return this;
							}
							PrepareBuilder();
							if (other.HasId)
							{
								SetId(other.Id);
							}
							if (other.HasNormalizedX)
							{
								SetNormalizedX(other.NormalizedX);
							}
							if (other.HasNormalizedY)
							{
								SetNormalizedY(other.NormalizedY);
							}
							return this;
						}

						public override Builder MergeFrom(ICodedInputStream input)
						{
							return MergeFrom(input, ExtensionRegistry.Empty);
						}

						public override Builder MergeFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
						{
							PrepareBuilder();
							uint fieldTag;
							string fieldName;
							while (input.ReadTag(out fieldTag, out fieldName))
							{
								if (fieldTag == 0 && fieldName != null)
								{
									int num = Array.BinarySearch(_pointerFieldNames, fieldName, StringComparer.Ordinal);
									if (num < 0)
									{
										ParseUnknownField(input, extensionRegistry, fieldTag, fieldName);
										continue;
									}
									fieldTag = _pointerFieldTags[num];
								}
								switch (fieldTag)
								{
								case 0u:
									throw InvalidProtocolBufferException.InvalidTag();
								case 8u:
									result.hasId = input.ReadInt32(ref result.id_);
									continue;
								case 21u:
									result.hasNormalizedX = input.ReadFloat(ref result.normalizedX_);
									continue;
								case 29u:
									result.hasNormalizedY = input.ReadFloat(ref result.normalizedY_);
									continue;
								}
								if (WireFormat.IsEndGroupTag(fieldTag))
								{
									return this;
								}
								ParseUnknownField(input, extensionRegistry, fieldTag, fieldName);
							}
							return this;
						}

						public Builder SetId(int value)
						{
							PrepareBuilder();
							result.hasId = true;
							result.id_ = value;
							return this;
						}

						public Builder ClearId()
						{
							PrepareBuilder();
							result.hasId = false;
							result.id_ = 0;
							return this;
						}

						public Builder SetNormalizedX(float value)
						{
							PrepareBuilder();
							result.hasNormalizedX = true;
							result.normalizedX_ = value;
							return this;
						}

						public Builder ClearNormalizedX()
						{
							PrepareBuilder();
							result.hasNormalizedX = false;
							result.normalizedX_ = 0f;
							return this;
						}

						public Builder SetNormalizedY(float value)
						{
							PrepareBuilder();
							result.hasNormalizedY = true;
							result.normalizedY_ = value;
							return this;
						}

						public Builder ClearNormalizedY()
						{
							PrepareBuilder();
							result.hasNormalizedY = false;
							result.normalizedY_ = 0f;
							return this;
						}
					}

					private static readonly Pointer defaultInstance;

					private static readonly string[] _pointerFieldNames;

					private static readonly uint[] _pointerFieldTags;

					public const int IdFieldNumber = 1;

					private bool hasId;

					private int id_;

					public const int NormalizedXFieldNumber = 2;

					private bool hasNormalizedX;

					private float normalizedX_;

					public const int NormalizedYFieldNumber = 3;

					private bool hasNormalizedY;

					private float normalizedY_;

					private int memoizedSerializedSize = -1;

					public static Pointer DefaultInstance => defaultInstance;

					public override Pointer DefaultInstanceForType => DefaultInstance;

					protected override Pointer ThisMessage => this;

					public bool HasId => hasId;

					public int Id => id_;

					public bool HasNormalizedX => hasNormalizedX;

					public float NormalizedX => normalizedX_;

					public bool HasNormalizedY => hasNormalizedY;

					public float NormalizedY => normalizedY_;

					public override bool IsInitialized => true;

					public override int SerializedSize
					{
						get
						{
							int num = memoizedSerializedSize;
							if (num != -1)
							{
								return num;
							}
							num = 0;
							if (hasId)
							{
								num += CodedOutputStream.ComputeInt32Size(1, Id);
							}
							if (hasNormalizedX)
							{
								num += CodedOutputStream.ComputeFloatSize(2, NormalizedX);
							}
							if (hasNormalizedY)
							{
								num += CodedOutputStream.ComputeFloatSize(3, NormalizedY);
							}
							memoizedSerializedSize = num;
							return num;
						}
					}

					private Pointer()
					{
					}

					static Pointer()
					{
						defaultInstance = new Pointer().MakeReadOnly();
						_pointerFieldNames = new string[3] { "id", "normalized_x", "normalized_y" };
						_pointerFieldTags = new uint[3] { 8u, 21u, 29u };
						object.ReferenceEquals(proto.Proto.PhoneEvent.Descriptor, null);
					}

					public override void WriteTo(ICodedOutputStream output)
					{
						string[] pointerFieldNames = _pointerFieldNames;
						if (hasId)
						{
							output.WriteInt32(1, pointerFieldNames[0], Id);
						}
						if (hasNormalizedX)
						{
							output.WriteFloat(2, pointerFieldNames[1], NormalizedX);
						}
						if (hasNormalizedY)
						{
							output.WriteFloat(3, pointerFieldNames[2], NormalizedY);
						}
					}

					public override int GetHashCode()
					{
						int num = GetType().GetHashCode();
						if (hasId)
						{
							num ^= id_.GetHashCode();
						}
						if (hasNormalizedX)
						{
							num ^= normalizedX_.GetHashCode();
						}
						if (hasNormalizedY)
						{
							num ^= normalizedY_.GetHashCode();
						}
						return num;
					}

					public override bool Equals(object obj)
					{
						if (!(obj is Pointer pointer))
						{
							return false;
						}
						if (hasId != pointer.hasId || (hasId && !id_.Equals(pointer.id_)))
						{
							return false;
						}
						if (hasNormalizedX != pointer.hasNormalizedX || (hasNormalizedX && !normalizedX_.Equals(pointer.normalizedX_)))
						{
							return false;
						}
						if (hasNormalizedY != pointer.hasNormalizedY || (hasNormalizedY && !normalizedY_.Equals(pointer.normalizedY_)))
						{
							return false;
						}
						return true;
					}

					public override void PrintTo(TextWriter writer)
					{
						GeneratedMessageLite<Pointer, Builder>.PrintField("id", hasId, id_, writer);
						GeneratedMessageLite<Pointer, Builder>.PrintField("normalized_x", hasNormalizedX, normalizedX_, writer);
						GeneratedMessageLite<Pointer, Builder>.PrintField("normalized_y", hasNormalizedY, normalizedY_, writer);
					}

					public static Pointer ParseFrom(ByteString data)
					{
						return CreateBuilder().MergeFrom(data).BuildParsed();
					}

					public static Pointer ParseFrom(ByteString data, ExtensionRegistry extensionRegistry)
					{
						return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
					}

					public static Pointer ParseFrom(byte[] data)
					{
						return CreateBuilder().MergeFrom(data).BuildParsed();
					}

					public static Pointer ParseFrom(byte[] data, ExtensionRegistry extensionRegistry)
					{
						return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
					}

					public static Pointer ParseFrom(Stream input)
					{
						return CreateBuilder().MergeFrom(input).BuildParsed();
					}

					public static Pointer ParseFrom(Stream input, ExtensionRegistry extensionRegistry)
					{
						return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
					}

					public static Pointer ParseDelimitedFrom(Stream input)
					{
						return CreateBuilder().MergeDelimitedFrom(input).BuildParsed();
					}

					public static Pointer ParseDelimitedFrom(Stream input, ExtensionRegistry extensionRegistry)
					{
						return CreateBuilder().MergeDelimitedFrom(input, extensionRegistry).BuildParsed();
					}

					public static Pointer ParseFrom(ICodedInputStream input)
					{
						return CreateBuilder().MergeFrom(input).BuildParsed();
					}

					public static Pointer ParseFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
					{
						return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
					}

					private Pointer MakeReadOnly()
					{
						return this;
					}

					public static Builder CreateBuilder()
					{
						return new Builder();
					}

					public override Builder ToBuilder()
					{
						return CreateBuilder(this);
					}

					public override Builder CreateBuilderForType()
					{
						return new Builder();
					}

					public static Builder CreateBuilder(Pointer prototype)
					{
						return new Builder(prototype);
					}
				}
			}

			[DebuggerNonUserCode]
			[CompilerGenerated]
			[GeneratedCode("ProtoGen", "2.4.1.473")]
			public sealed class Builder : GeneratedBuilderLite<MotionEvent, Builder>
			{
				private bool resultIsReadOnly;

				private MotionEvent result;

				protected override Builder ThisBuilder => this;

				public override bool IsInitialized => result.IsInitialized;

				protected override MotionEvent MessageBeingBuilt => PrepareBuilder();

				public override MotionEvent DefaultInstanceForType => DefaultInstance;

				public bool HasTimestamp => result.hasTimestamp;

				public long Timestamp
				{
					get
					{
						return result.Timestamp;
					}
					set
					{
						SetTimestamp(value);
					}
				}

				public bool HasAction => result.hasAction;

				public int Action
				{
					get
					{
						return result.Action;
					}
					set
					{
						SetAction(value);
					}
				}

				public IPopsicleList<Types.Pointer> PointersList => PrepareBuilder().pointers_;

				public int PointersCount => result.PointersCount;

				public Builder()
				{
					result = DefaultInstance;
					resultIsReadOnly = true;
				}

				internal Builder(MotionEvent cloneFrom)
				{
					result = cloneFrom;
					resultIsReadOnly = true;
				}

				private MotionEvent PrepareBuilder()
				{
					if (resultIsReadOnly)
					{
						MotionEvent other = result;
						result = new MotionEvent();
						resultIsReadOnly = false;
						MergeFrom(other);
					}
					return result;
				}

				public override Builder Clear()
				{
					result = DefaultInstance;
					resultIsReadOnly = true;
					return this;
				}

				public override Builder Clone()
				{
					if (resultIsReadOnly)
					{
						return new Builder(result);
					}
					return new Builder().MergeFrom(result);
				}

				public override MotionEvent BuildPartial()
				{
					if (resultIsReadOnly)
					{
						return result;
					}
					resultIsReadOnly = true;
					return result.MakeReadOnly();
				}

				public override Builder MergeFrom(IMessageLite other)
				{
					if (other is MotionEvent)
					{
						return MergeFrom((MotionEvent)other);
					}
					base.MergeFrom(other);
					return this;
				}

				public override Builder MergeFrom(MotionEvent other)
				{
					if (other == DefaultInstance)
					{
						return this;
					}
					PrepareBuilder();
					if (other.HasTimestamp)
					{
						SetTimestamp(other.Timestamp);
					}
					if (other.HasAction)
					{
						SetAction(other.Action);
					}
					if (other.pointers_.Count != 0)
					{
						result.pointers_.Add(other.pointers_);
					}
					return this;
				}

				public override Builder MergeFrom(ICodedInputStream input)
				{
					return MergeFrom(input, ExtensionRegistry.Empty);
				}

				public override Builder MergeFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
				{
					PrepareBuilder();
					uint fieldTag;
					string fieldName;
					while (input.ReadTag(out fieldTag, out fieldName))
					{
						if (fieldTag == 0 && fieldName != null)
						{
							int num = Array.BinarySearch(_motionEventFieldNames, fieldName, StringComparer.Ordinal);
							if (num < 0)
							{
								ParseUnknownField(input, extensionRegistry, fieldTag, fieldName);
								continue;
							}
							fieldTag = _motionEventFieldTags[num];
						}
						switch (fieldTag)
						{
						case 0u:
							throw InvalidProtocolBufferException.InvalidTag();
						case 8u:
							result.hasTimestamp = input.ReadInt64(ref result.timestamp_);
							continue;
						case 16u:
							result.hasAction = input.ReadInt32(ref result.action_);
							continue;
						case 26u:
							input.ReadMessageArray(fieldTag, fieldName, result.pointers_, Types.Pointer.DefaultInstance, extensionRegistry);
							continue;
						}
						if (WireFormat.IsEndGroupTag(fieldTag))
						{
							return this;
						}
						ParseUnknownField(input, extensionRegistry, fieldTag, fieldName);
					}
					return this;
				}

				public Builder SetTimestamp(long value)
				{
					PrepareBuilder();
					result.hasTimestamp = true;
					result.timestamp_ = value;
					return this;
				}

				public Builder ClearTimestamp()
				{
					PrepareBuilder();
					result.hasTimestamp = false;
					result.timestamp_ = 0L;
					return this;
				}

				public Builder SetAction(int value)
				{
					PrepareBuilder();
					result.hasAction = true;
					result.action_ = value;
					return this;
				}

				public Builder ClearAction()
				{
					PrepareBuilder();
					result.hasAction = false;
					result.action_ = 0;
					return this;
				}

				public Types.Pointer GetPointers(int index)
				{
					return result.GetPointers(index);
				}

				public Builder SetPointers(int index, Types.Pointer value)
				{
					ThrowHelper.ThrowIfNull(value, "value");
					PrepareBuilder();
					result.pointers_[index] = value;
					return this;
				}

				public Builder SetPointers(int index, Types.Pointer.Builder builderForValue)
				{
					ThrowHelper.ThrowIfNull(builderForValue, "builderForValue");
					PrepareBuilder();
					result.pointers_[index] = builderForValue.Build();
					return this;
				}

				public Builder AddPointers(Types.Pointer value)
				{
					ThrowHelper.ThrowIfNull(value, "value");
					PrepareBuilder();
					result.pointers_.Add(value);
					return this;
				}

				public Builder AddPointers(Types.Pointer.Builder builderForValue)
				{
					ThrowHelper.ThrowIfNull(builderForValue, "builderForValue");
					PrepareBuilder();
					result.pointers_.Add(builderForValue.Build());
					return this;
				}

				public Builder AddRangePointers(IEnumerable<Types.Pointer> values)
				{
					PrepareBuilder();
					result.pointers_.Add(values);
					return this;
				}

				public Builder ClearPointers()
				{
					PrepareBuilder();
					result.pointers_.Clear();
					return this;
				}
			}

			private static readonly MotionEvent defaultInstance;

			private static readonly string[] _motionEventFieldNames;

			private static readonly uint[] _motionEventFieldTags;

			public const int TimestampFieldNumber = 1;

			private bool hasTimestamp;

			private long timestamp_;

			public const int ActionFieldNumber = 2;

			private bool hasAction;

			private int action_;

			public const int PointersFieldNumber = 3;

			private PopsicleList<Types.Pointer> pointers_ = new PopsicleList<Types.Pointer>();

			private int memoizedSerializedSize = -1;

			public static MotionEvent DefaultInstance => defaultInstance;

			public override MotionEvent DefaultInstanceForType => DefaultInstance;

			protected override MotionEvent ThisMessage => this;

			public bool HasTimestamp => hasTimestamp;

			public long Timestamp => timestamp_;

			public bool HasAction => hasAction;

			public int Action => action_;

			public IList<Types.Pointer> PointersList => pointers_;

			public int PointersCount => pointers_.Count;

			public override bool IsInitialized => true;

			public override int SerializedSize
			{
				get
				{
					int num = memoizedSerializedSize;
					if (num != -1)
					{
						return num;
					}
					num = 0;
					if (hasTimestamp)
					{
						num += CodedOutputStream.ComputeInt64Size(1, Timestamp);
					}
					if (hasAction)
					{
						num += CodedOutputStream.ComputeInt32Size(2, Action);
					}
					foreach (Types.Pointer pointers in PointersList)
					{
						num += CodedOutputStream.ComputeMessageSize(3, pointers);
					}
					memoizedSerializedSize = num;
					return num;
				}
			}

			private MotionEvent()
			{
			}

			static MotionEvent()
			{
				defaultInstance = new MotionEvent().MakeReadOnly();
				_motionEventFieldNames = new string[3] { "action", "pointers", "timestamp" };
				_motionEventFieldTags = new uint[3] { 16u, 26u, 8u };
				object.ReferenceEquals(proto.Proto.PhoneEvent.Descriptor, null);
			}

			public Types.Pointer GetPointers(int index)
			{
				return pointers_[index];
			}

			public override void WriteTo(ICodedOutputStream output)
			{
				string[] motionEventFieldNames = _motionEventFieldNames;
				if (hasTimestamp)
				{
					output.WriteInt64(1, motionEventFieldNames[2], Timestamp);
				}
				if (hasAction)
				{
					output.WriteInt32(2, motionEventFieldNames[0], Action);
				}
				if (pointers_.Count > 0)
				{
					output.WriteMessageArray(3, motionEventFieldNames[1], pointers_);
				}
			}

			public override int GetHashCode()
			{
				int num = GetType().GetHashCode();
				if (hasTimestamp)
				{
					num ^= timestamp_.GetHashCode();
				}
				if (hasAction)
				{
					num ^= action_.GetHashCode();
				}
				foreach (Types.Pointer item in pointers_)
				{
					num ^= item.GetHashCode();
				}
				return num;
			}

			public override bool Equals(object obj)
			{
				if (!(obj is MotionEvent motionEvent))
				{
					return false;
				}
				if (hasTimestamp != motionEvent.hasTimestamp || (hasTimestamp && !timestamp_.Equals(motionEvent.timestamp_)))
				{
					return false;
				}
				if (hasAction != motionEvent.hasAction || (hasAction && !action_.Equals(motionEvent.action_)))
				{
					return false;
				}
				if (pointers_.Count != motionEvent.pointers_.Count)
				{
					return false;
				}
				for (int i = 0; i < pointers_.Count; i++)
				{
					if (!pointers_[i].Equals(motionEvent.pointers_[i]))
					{
						return false;
					}
				}
				return true;
			}

			public override void PrintTo(TextWriter writer)
			{
				GeneratedMessageLite<MotionEvent, Builder>.PrintField("timestamp", hasTimestamp, timestamp_, writer);
				GeneratedMessageLite<MotionEvent, Builder>.PrintField("action", hasAction, action_, writer);
				GeneratedMessageLite<MotionEvent, Builder>.PrintField("pointers", pointers_, writer);
			}

			public static MotionEvent ParseFrom(ByteString data)
			{
				return CreateBuilder().MergeFrom(data).BuildParsed();
			}

			public static MotionEvent ParseFrom(ByteString data, ExtensionRegistry extensionRegistry)
			{
				return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
			}

			public static MotionEvent ParseFrom(byte[] data)
			{
				return CreateBuilder().MergeFrom(data).BuildParsed();
			}

			public static MotionEvent ParseFrom(byte[] data, ExtensionRegistry extensionRegistry)
			{
				return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
			}

			public static MotionEvent ParseFrom(Stream input)
			{
				return CreateBuilder().MergeFrom(input).BuildParsed();
			}

			public static MotionEvent ParseFrom(Stream input, ExtensionRegistry extensionRegistry)
			{
				return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
			}

			public static MotionEvent ParseDelimitedFrom(Stream input)
			{
				return CreateBuilder().MergeDelimitedFrom(input).BuildParsed();
			}

			public static MotionEvent ParseDelimitedFrom(Stream input, ExtensionRegistry extensionRegistry)
			{
				return CreateBuilder().MergeDelimitedFrom(input, extensionRegistry).BuildParsed();
			}

			public static MotionEvent ParseFrom(ICodedInputStream input)
			{
				return CreateBuilder().MergeFrom(input).BuildParsed();
			}

			public static MotionEvent ParseFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
			{
				return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
			}

			private MotionEvent MakeReadOnly()
			{
				pointers_.MakeReadOnly();
				return this;
			}

			public static Builder CreateBuilder()
			{
				return new Builder();
			}

			public override Builder ToBuilder()
			{
				return CreateBuilder(this);
			}

			public override Builder CreateBuilderForType()
			{
				return new Builder();
			}

			public static Builder CreateBuilder(MotionEvent prototype)
			{
				return new Builder(prototype);
			}
		}

		[DebuggerNonUserCode]
		[CompilerGenerated]
		[GeneratedCode("ProtoGen", "2.4.1.473")]
		public sealed class GyroscopeEvent : GeneratedMessageLite<GyroscopeEvent, GyroscopeEvent.Builder>
		{
			[DebuggerNonUserCode]
			[CompilerGenerated]
			[GeneratedCode("ProtoGen", "2.4.1.473")]
			public sealed class Builder : GeneratedBuilderLite<GyroscopeEvent, Builder>
			{
				private bool resultIsReadOnly;

				private GyroscopeEvent result;

				protected override Builder ThisBuilder => this;

				public override bool IsInitialized => result.IsInitialized;

				protected override GyroscopeEvent MessageBeingBuilt => PrepareBuilder();

				public override GyroscopeEvent DefaultInstanceForType => DefaultInstance;

				public bool HasTimestamp => result.hasTimestamp;

				public long Timestamp
				{
					get
					{
						return result.Timestamp;
					}
					set
					{
						SetTimestamp(value);
					}
				}

				public bool HasX => result.hasX;

				public float X
				{
					get
					{
						return result.X;
					}
					set
					{
						SetX(value);
					}
				}

				public bool HasY => result.hasY;

				public float Y
				{
					get
					{
						return result.Y;
					}
					set
					{
						SetY(value);
					}
				}

				public bool HasZ => result.hasZ;

				public float Z
				{
					get
					{
						return result.Z;
					}
					set
					{
						SetZ(value);
					}
				}

				public Builder()
				{
					result = DefaultInstance;
					resultIsReadOnly = true;
				}

				internal Builder(GyroscopeEvent cloneFrom)
				{
					result = cloneFrom;
					resultIsReadOnly = true;
				}

				private GyroscopeEvent PrepareBuilder()
				{
					if (resultIsReadOnly)
					{
						GyroscopeEvent other = result;
						result = new GyroscopeEvent();
						resultIsReadOnly = false;
						MergeFrom(other);
					}
					return result;
				}

				public override Builder Clear()
				{
					result = DefaultInstance;
					resultIsReadOnly = true;
					return this;
				}

				public override Builder Clone()
				{
					if (resultIsReadOnly)
					{
						return new Builder(result);
					}
					return new Builder().MergeFrom(result);
				}

				public override GyroscopeEvent BuildPartial()
				{
					if (resultIsReadOnly)
					{
						return result;
					}
					resultIsReadOnly = true;
					return result.MakeReadOnly();
				}

				public override Builder MergeFrom(IMessageLite other)
				{
					if (other is GyroscopeEvent)
					{
						return MergeFrom((GyroscopeEvent)other);
					}
					base.MergeFrom(other);
					return this;
				}

				public override Builder MergeFrom(GyroscopeEvent other)
				{
					if (other == DefaultInstance)
					{
						return this;
					}
					PrepareBuilder();
					if (other.HasTimestamp)
					{
						SetTimestamp(other.Timestamp);
					}
					if (other.HasX)
					{
						SetX(other.X);
					}
					if (other.HasY)
					{
						SetY(other.Y);
					}
					if (other.HasZ)
					{
						SetZ(other.Z);
					}
					return this;
				}

				public override Builder MergeFrom(ICodedInputStream input)
				{
					return MergeFrom(input, ExtensionRegistry.Empty);
				}

				public override Builder MergeFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
				{
					PrepareBuilder();
					uint fieldTag;
					string fieldName;
					while (input.ReadTag(out fieldTag, out fieldName))
					{
						if (fieldTag == 0 && fieldName != null)
						{
							int num = Array.BinarySearch(_gyroscopeEventFieldNames, fieldName, StringComparer.Ordinal);
							if (num < 0)
							{
								ParseUnknownField(input, extensionRegistry, fieldTag, fieldName);
								continue;
							}
							fieldTag = _gyroscopeEventFieldTags[num];
						}
						switch (fieldTag)
						{
						case 0u:
							throw InvalidProtocolBufferException.InvalidTag();
						case 8u:
							result.hasTimestamp = input.ReadInt64(ref result.timestamp_);
							continue;
						case 21u:
							result.hasX = input.ReadFloat(ref result.x_);
							continue;
						case 29u:
							result.hasY = input.ReadFloat(ref result.y_);
							continue;
						case 37u:
							result.hasZ = input.ReadFloat(ref result.z_);
							continue;
						}
						if (WireFormat.IsEndGroupTag(fieldTag))
						{
							return this;
						}
						ParseUnknownField(input, extensionRegistry, fieldTag, fieldName);
					}
					return this;
				}

				public Builder SetTimestamp(long value)
				{
					PrepareBuilder();
					result.hasTimestamp = true;
					result.timestamp_ = value;
					return this;
				}

				public Builder ClearTimestamp()
				{
					PrepareBuilder();
					result.hasTimestamp = false;
					result.timestamp_ = 0L;
					return this;
				}

				public Builder SetX(float value)
				{
					PrepareBuilder();
					result.hasX = true;
					result.x_ = value;
					return this;
				}

				public Builder ClearX()
				{
					PrepareBuilder();
					result.hasX = false;
					result.x_ = 0f;
					return this;
				}

				public Builder SetY(float value)
				{
					PrepareBuilder();
					result.hasY = true;
					result.y_ = value;
					return this;
				}

				public Builder ClearY()
				{
					PrepareBuilder();
					result.hasY = false;
					result.y_ = 0f;
					return this;
				}

				public Builder SetZ(float value)
				{
					PrepareBuilder();
					result.hasZ = true;
					result.z_ = value;
					return this;
				}

				public Builder ClearZ()
				{
					PrepareBuilder();
					result.hasZ = false;
					result.z_ = 0f;
					return this;
				}
			}

			private static readonly GyroscopeEvent defaultInstance;

			private static readonly string[] _gyroscopeEventFieldNames;

			private static readonly uint[] _gyroscopeEventFieldTags;

			public const int TimestampFieldNumber = 1;

			private bool hasTimestamp;

			private long timestamp_;

			public const int XFieldNumber = 2;

			private bool hasX;

			private float x_;

			public const int YFieldNumber = 3;

			private bool hasY;

			private float y_;

			public const int ZFieldNumber = 4;

			private bool hasZ;

			private float z_;

			private int memoizedSerializedSize = -1;

			public static GyroscopeEvent DefaultInstance => defaultInstance;

			public override GyroscopeEvent DefaultInstanceForType => DefaultInstance;

			protected override GyroscopeEvent ThisMessage => this;

			public bool HasTimestamp => hasTimestamp;

			public long Timestamp => timestamp_;

			public bool HasX => hasX;

			public float X => x_;

			public bool HasY => hasY;

			public float Y => y_;

			public bool HasZ => hasZ;

			public float Z => z_;

			public override bool IsInitialized => true;

			public override int SerializedSize
			{
				get
				{
					int num = memoizedSerializedSize;
					if (num != -1)
					{
						return num;
					}
					num = 0;
					if (hasTimestamp)
					{
						num += CodedOutputStream.ComputeInt64Size(1, Timestamp);
					}
					if (hasX)
					{
						num += CodedOutputStream.ComputeFloatSize(2, X);
					}
					if (hasY)
					{
						num += CodedOutputStream.ComputeFloatSize(3, Y);
					}
					if (hasZ)
					{
						num += CodedOutputStream.ComputeFloatSize(4, Z);
					}
					memoizedSerializedSize = num;
					return num;
				}
			}

			private GyroscopeEvent()
			{
			}

			static GyroscopeEvent()
			{
				defaultInstance = new GyroscopeEvent().MakeReadOnly();
				_gyroscopeEventFieldNames = new string[4] { "timestamp", "x", "y", "z" };
				_gyroscopeEventFieldTags = new uint[4] { 8u, 21u, 29u, 37u };
				object.ReferenceEquals(proto.Proto.PhoneEvent.Descriptor, null);
			}

			public override void WriteTo(ICodedOutputStream output)
			{
				string[] gyroscopeEventFieldNames = _gyroscopeEventFieldNames;
				if (hasTimestamp)
				{
					output.WriteInt64(1, gyroscopeEventFieldNames[0], Timestamp);
				}
				if (hasX)
				{
					output.WriteFloat(2, gyroscopeEventFieldNames[1], X);
				}
				if (hasY)
				{
					output.WriteFloat(3, gyroscopeEventFieldNames[2], Y);
				}
				if (hasZ)
				{
					output.WriteFloat(4, gyroscopeEventFieldNames[3], Z);
				}
			}

			public override int GetHashCode()
			{
				int num = GetType().GetHashCode();
				if (hasTimestamp)
				{
					num ^= timestamp_.GetHashCode();
				}
				if (hasX)
				{
					num ^= x_.GetHashCode();
				}
				if (hasY)
				{
					num ^= y_.GetHashCode();
				}
				if (hasZ)
				{
					num ^= z_.GetHashCode();
				}
				return num;
			}

			public override bool Equals(object obj)
			{
				if (!(obj is GyroscopeEvent gyroscopeEvent))
				{
					return false;
				}
				if (hasTimestamp != gyroscopeEvent.hasTimestamp || (hasTimestamp && !timestamp_.Equals(gyroscopeEvent.timestamp_)))
				{
					return false;
				}
				if (hasX != gyroscopeEvent.hasX || (hasX && !x_.Equals(gyroscopeEvent.x_)))
				{
					return false;
				}
				if (hasY != gyroscopeEvent.hasY || (hasY && !y_.Equals(gyroscopeEvent.y_)))
				{
					return false;
				}
				if (hasZ != gyroscopeEvent.hasZ || (hasZ && !z_.Equals(gyroscopeEvent.z_)))
				{
					return false;
				}
				return true;
			}

			public override void PrintTo(TextWriter writer)
			{
				GeneratedMessageLite<GyroscopeEvent, Builder>.PrintField("timestamp", hasTimestamp, timestamp_, writer);
				GeneratedMessageLite<GyroscopeEvent, Builder>.PrintField("x", hasX, x_, writer);
				GeneratedMessageLite<GyroscopeEvent, Builder>.PrintField("y", hasY, y_, writer);
				GeneratedMessageLite<GyroscopeEvent, Builder>.PrintField("z", hasZ, z_, writer);
			}

			public static GyroscopeEvent ParseFrom(ByteString data)
			{
				return CreateBuilder().MergeFrom(data).BuildParsed();
			}

			public static GyroscopeEvent ParseFrom(ByteString data, ExtensionRegistry extensionRegistry)
			{
				return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
			}

			public static GyroscopeEvent ParseFrom(byte[] data)
			{
				return CreateBuilder().MergeFrom(data).BuildParsed();
			}

			public static GyroscopeEvent ParseFrom(byte[] data, ExtensionRegistry extensionRegistry)
			{
				return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
			}

			public static GyroscopeEvent ParseFrom(Stream input)
			{
				return CreateBuilder().MergeFrom(input).BuildParsed();
			}

			public static GyroscopeEvent ParseFrom(Stream input, ExtensionRegistry extensionRegistry)
			{
				return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
			}

			public static GyroscopeEvent ParseDelimitedFrom(Stream input)
			{
				return CreateBuilder().MergeDelimitedFrom(input).BuildParsed();
			}

			public static GyroscopeEvent ParseDelimitedFrom(Stream input, ExtensionRegistry extensionRegistry)
			{
				return CreateBuilder().MergeDelimitedFrom(input, extensionRegistry).BuildParsed();
			}

			public static GyroscopeEvent ParseFrom(ICodedInputStream input)
			{
				return CreateBuilder().MergeFrom(input).BuildParsed();
			}

			public static GyroscopeEvent ParseFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
			{
				return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
			}

			private GyroscopeEvent MakeReadOnly()
			{
				return this;
			}

			public static Builder CreateBuilder()
			{
				return new Builder();
			}

			public override Builder ToBuilder()
			{
				return CreateBuilder(this);
			}

			public override Builder CreateBuilderForType()
			{
				return new Builder();
			}

			public static Builder CreateBuilder(GyroscopeEvent prototype)
			{
				return new Builder(prototype);
			}
		}

		[DebuggerNonUserCode]
		[CompilerGenerated]
		[GeneratedCode("ProtoGen", "2.4.1.473")]
		public sealed class AccelerometerEvent : GeneratedMessageLite<AccelerometerEvent, AccelerometerEvent.Builder>
		{
			[DebuggerNonUserCode]
			[CompilerGenerated]
			[GeneratedCode("ProtoGen", "2.4.1.473")]
			public sealed class Builder : GeneratedBuilderLite<AccelerometerEvent, Builder>
			{
				private bool resultIsReadOnly;

				private AccelerometerEvent result;

				protected override Builder ThisBuilder => this;

				public override bool IsInitialized => result.IsInitialized;

				protected override AccelerometerEvent MessageBeingBuilt => PrepareBuilder();

				public override AccelerometerEvent DefaultInstanceForType => DefaultInstance;

				public bool HasTimestamp => result.hasTimestamp;

				public long Timestamp
				{
					get
					{
						return result.Timestamp;
					}
					set
					{
						SetTimestamp(value);
					}
				}

				public bool HasX => result.hasX;

				public float X
				{
					get
					{
						return result.X;
					}
					set
					{
						SetX(value);
					}
				}

				public bool HasY => result.hasY;

				public float Y
				{
					get
					{
						return result.Y;
					}
					set
					{
						SetY(value);
					}
				}

				public bool HasZ => result.hasZ;

				public float Z
				{
					get
					{
						return result.Z;
					}
					set
					{
						SetZ(value);
					}
				}

				public Builder()
				{
					result = DefaultInstance;
					resultIsReadOnly = true;
				}

				internal Builder(AccelerometerEvent cloneFrom)
				{
					result = cloneFrom;
					resultIsReadOnly = true;
				}

				private AccelerometerEvent PrepareBuilder()
				{
					if (resultIsReadOnly)
					{
						AccelerometerEvent other = result;
						result = new AccelerometerEvent();
						resultIsReadOnly = false;
						MergeFrom(other);
					}
					return result;
				}

				public override Builder Clear()
				{
					result = DefaultInstance;
					resultIsReadOnly = true;
					return this;
				}

				public override Builder Clone()
				{
					if (resultIsReadOnly)
					{
						return new Builder(result);
					}
					return new Builder().MergeFrom(result);
				}

				public override AccelerometerEvent BuildPartial()
				{
					if (resultIsReadOnly)
					{
						return result;
					}
					resultIsReadOnly = true;
					return result.MakeReadOnly();
				}

				public override Builder MergeFrom(IMessageLite other)
				{
					if (other is AccelerometerEvent)
					{
						return MergeFrom((AccelerometerEvent)other);
					}
					base.MergeFrom(other);
					return this;
				}

				public override Builder MergeFrom(AccelerometerEvent other)
				{
					if (other == DefaultInstance)
					{
						return this;
					}
					PrepareBuilder();
					if (other.HasTimestamp)
					{
						SetTimestamp(other.Timestamp);
					}
					if (other.HasX)
					{
						SetX(other.X);
					}
					if (other.HasY)
					{
						SetY(other.Y);
					}
					if (other.HasZ)
					{
						SetZ(other.Z);
					}
					return this;
				}

				public override Builder MergeFrom(ICodedInputStream input)
				{
					return MergeFrom(input, ExtensionRegistry.Empty);
				}

				public override Builder MergeFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
				{
					PrepareBuilder();
					uint fieldTag;
					string fieldName;
					while (input.ReadTag(out fieldTag, out fieldName))
					{
						if (fieldTag == 0 && fieldName != null)
						{
							int num = Array.BinarySearch(_accelerometerEventFieldNames, fieldName, StringComparer.Ordinal);
							if (num < 0)
							{
								ParseUnknownField(input, extensionRegistry, fieldTag, fieldName);
								continue;
							}
							fieldTag = _accelerometerEventFieldTags[num];
						}
						switch (fieldTag)
						{
						case 0u:
							throw InvalidProtocolBufferException.InvalidTag();
						case 8u:
							result.hasTimestamp = input.ReadInt64(ref result.timestamp_);
							continue;
						case 21u:
							result.hasX = input.ReadFloat(ref result.x_);
							continue;
						case 29u:
							result.hasY = input.ReadFloat(ref result.y_);
							continue;
						case 37u:
							result.hasZ = input.ReadFloat(ref result.z_);
							continue;
						}
						if (WireFormat.IsEndGroupTag(fieldTag))
						{
							return this;
						}
						ParseUnknownField(input, extensionRegistry, fieldTag, fieldName);
					}
					return this;
				}

				public Builder SetTimestamp(long value)
				{
					PrepareBuilder();
					result.hasTimestamp = true;
					result.timestamp_ = value;
					return this;
				}

				public Builder ClearTimestamp()
				{
					PrepareBuilder();
					result.hasTimestamp = false;
					result.timestamp_ = 0L;
					return this;
				}

				public Builder SetX(float value)
				{
					PrepareBuilder();
					result.hasX = true;
					result.x_ = value;
					return this;
				}

				public Builder ClearX()
				{
					PrepareBuilder();
					result.hasX = false;
					result.x_ = 0f;
					return this;
				}

				public Builder SetY(float value)
				{
					PrepareBuilder();
					result.hasY = true;
					result.y_ = value;
					return this;
				}

				public Builder ClearY()
				{
					PrepareBuilder();
					result.hasY = false;
					result.y_ = 0f;
					return this;
				}

				public Builder SetZ(float value)
				{
					PrepareBuilder();
					result.hasZ = true;
					result.z_ = value;
					return this;
				}

				public Builder ClearZ()
				{
					PrepareBuilder();
					result.hasZ = false;
					result.z_ = 0f;
					return this;
				}
			}

			private static readonly AccelerometerEvent defaultInstance;

			private static readonly string[] _accelerometerEventFieldNames;

			private static readonly uint[] _accelerometerEventFieldTags;

			public const int TimestampFieldNumber = 1;

			private bool hasTimestamp;

			private long timestamp_;

			public const int XFieldNumber = 2;

			private bool hasX;

			private float x_;

			public const int YFieldNumber = 3;

			private bool hasY;

			private float y_;

			public const int ZFieldNumber = 4;

			private bool hasZ;

			private float z_;

			private int memoizedSerializedSize = -1;

			public static AccelerometerEvent DefaultInstance => defaultInstance;

			public override AccelerometerEvent DefaultInstanceForType => DefaultInstance;

			protected override AccelerometerEvent ThisMessage => this;

			public bool HasTimestamp => hasTimestamp;

			public long Timestamp => timestamp_;

			public bool HasX => hasX;

			public float X => x_;

			public bool HasY => hasY;

			public float Y => y_;

			public bool HasZ => hasZ;

			public float Z => z_;

			public override bool IsInitialized => true;

			public override int SerializedSize
			{
				get
				{
					int num = memoizedSerializedSize;
					if (num != -1)
					{
						return num;
					}
					num = 0;
					if (hasTimestamp)
					{
						num += CodedOutputStream.ComputeInt64Size(1, Timestamp);
					}
					if (hasX)
					{
						num += CodedOutputStream.ComputeFloatSize(2, X);
					}
					if (hasY)
					{
						num += CodedOutputStream.ComputeFloatSize(3, Y);
					}
					if (hasZ)
					{
						num += CodedOutputStream.ComputeFloatSize(4, Z);
					}
					memoizedSerializedSize = num;
					return num;
				}
			}

			private AccelerometerEvent()
			{
			}

			static AccelerometerEvent()
			{
				defaultInstance = new AccelerometerEvent().MakeReadOnly();
				_accelerometerEventFieldNames = new string[4] { "timestamp", "x", "y", "z" };
				_accelerometerEventFieldTags = new uint[4] { 8u, 21u, 29u, 37u };
				object.ReferenceEquals(proto.Proto.PhoneEvent.Descriptor, null);
			}

			public override void WriteTo(ICodedOutputStream output)
			{
				string[] accelerometerEventFieldNames = _accelerometerEventFieldNames;
				if (hasTimestamp)
				{
					output.WriteInt64(1, accelerometerEventFieldNames[0], Timestamp);
				}
				if (hasX)
				{
					output.WriteFloat(2, accelerometerEventFieldNames[1], X);
				}
				if (hasY)
				{
					output.WriteFloat(3, accelerometerEventFieldNames[2], Y);
				}
				if (hasZ)
				{
					output.WriteFloat(4, accelerometerEventFieldNames[3], Z);
				}
			}

			public override int GetHashCode()
			{
				int num = GetType().GetHashCode();
				if (hasTimestamp)
				{
					num ^= timestamp_.GetHashCode();
				}
				if (hasX)
				{
					num ^= x_.GetHashCode();
				}
				if (hasY)
				{
					num ^= y_.GetHashCode();
				}
				if (hasZ)
				{
					num ^= z_.GetHashCode();
				}
				return num;
			}

			public override bool Equals(object obj)
			{
				if (!(obj is AccelerometerEvent accelerometerEvent))
				{
					return false;
				}
				if (hasTimestamp != accelerometerEvent.hasTimestamp || (hasTimestamp && !timestamp_.Equals(accelerometerEvent.timestamp_)))
				{
					return false;
				}
				if (hasX != accelerometerEvent.hasX || (hasX && !x_.Equals(accelerometerEvent.x_)))
				{
					return false;
				}
				if (hasY != accelerometerEvent.hasY || (hasY && !y_.Equals(accelerometerEvent.y_)))
				{
					return false;
				}
				if (hasZ != accelerometerEvent.hasZ || (hasZ && !z_.Equals(accelerometerEvent.z_)))
				{
					return false;
				}
				return true;
			}

			public override void PrintTo(TextWriter writer)
			{
				GeneratedMessageLite<AccelerometerEvent, Builder>.PrintField("timestamp", hasTimestamp, timestamp_, writer);
				GeneratedMessageLite<AccelerometerEvent, Builder>.PrintField("x", hasX, x_, writer);
				GeneratedMessageLite<AccelerometerEvent, Builder>.PrintField("y", hasY, y_, writer);
				GeneratedMessageLite<AccelerometerEvent, Builder>.PrintField("z", hasZ, z_, writer);
			}

			public static AccelerometerEvent ParseFrom(ByteString data)
			{
				return CreateBuilder().MergeFrom(data).BuildParsed();
			}

			public static AccelerometerEvent ParseFrom(ByteString data, ExtensionRegistry extensionRegistry)
			{
				return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
			}

			public static AccelerometerEvent ParseFrom(byte[] data)
			{
				return CreateBuilder().MergeFrom(data).BuildParsed();
			}

			public static AccelerometerEvent ParseFrom(byte[] data, ExtensionRegistry extensionRegistry)
			{
				return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
			}

			public static AccelerometerEvent ParseFrom(Stream input)
			{
				return CreateBuilder().MergeFrom(input).BuildParsed();
			}

			public static AccelerometerEvent ParseFrom(Stream input, ExtensionRegistry extensionRegistry)
			{
				return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
			}

			public static AccelerometerEvent ParseDelimitedFrom(Stream input)
			{
				return CreateBuilder().MergeDelimitedFrom(input).BuildParsed();
			}

			public static AccelerometerEvent ParseDelimitedFrom(Stream input, ExtensionRegistry extensionRegistry)
			{
				return CreateBuilder().MergeDelimitedFrom(input, extensionRegistry).BuildParsed();
			}

			public static AccelerometerEvent ParseFrom(ICodedInputStream input)
			{
				return CreateBuilder().MergeFrom(input).BuildParsed();
			}

			public static AccelerometerEvent ParseFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
			{
				return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
			}

			private AccelerometerEvent MakeReadOnly()
			{
				return this;
			}

			public static Builder CreateBuilder()
			{
				return new Builder();
			}

			public override Builder ToBuilder()
			{
				return CreateBuilder(this);
			}

			public override Builder CreateBuilderForType()
			{
				return new Builder();
			}

			public static Builder CreateBuilder(AccelerometerEvent prototype)
			{
				return new Builder(prototype);
			}
		}

		[DebuggerNonUserCode]
		[CompilerGenerated]
		[GeneratedCode("ProtoGen", "2.4.1.473")]
		public sealed class DepthMapEvent : GeneratedMessageLite<DepthMapEvent, DepthMapEvent.Builder>
		{
			[DebuggerNonUserCode]
			[CompilerGenerated]
			[GeneratedCode("ProtoGen", "2.4.1.473")]
			public sealed class Builder : GeneratedBuilderLite<DepthMapEvent, Builder>
			{
				private bool resultIsReadOnly;

				private DepthMapEvent result;

				protected override Builder ThisBuilder => this;

				public override bool IsInitialized => result.IsInitialized;

				protected override DepthMapEvent MessageBeingBuilt => PrepareBuilder();

				public override DepthMapEvent DefaultInstanceForType => DefaultInstance;

				public bool HasTimestamp => result.hasTimestamp;

				public long Timestamp
				{
					get
					{
						return result.Timestamp;
					}
					set
					{
						SetTimestamp(value);
					}
				}

				public bool HasWidth => result.hasWidth;

				public int Width
				{
					get
					{
						return result.Width;
					}
					set
					{
						SetWidth(value);
					}
				}

				public bool HasHeight => result.hasHeight;

				public int Height
				{
					get
					{
						return result.Height;
					}
					set
					{
						SetHeight(value);
					}
				}

				public IPopsicleList<float> ZDistancesList => PrepareBuilder().zDistances_;

				public int ZDistancesCount => result.ZDistancesCount;

				public Builder()
				{
					result = DefaultInstance;
					resultIsReadOnly = true;
				}

				internal Builder(DepthMapEvent cloneFrom)
				{
					result = cloneFrom;
					resultIsReadOnly = true;
				}

				private DepthMapEvent PrepareBuilder()
				{
					if (resultIsReadOnly)
					{
						DepthMapEvent other = result;
						result = new DepthMapEvent();
						resultIsReadOnly = false;
						MergeFrom(other);
					}
					return result;
				}

				public override Builder Clear()
				{
					result = DefaultInstance;
					resultIsReadOnly = true;
					return this;
				}

				public override Builder Clone()
				{
					if (resultIsReadOnly)
					{
						return new Builder(result);
					}
					return new Builder().MergeFrom(result);
				}

				public override DepthMapEvent BuildPartial()
				{
					if (resultIsReadOnly)
					{
						return result;
					}
					resultIsReadOnly = true;
					return result.MakeReadOnly();
				}

				public override Builder MergeFrom(IMessageLite other)
				{
					if (other is DepthMapEvent)
					{
						return MergeFrom((DepthMapEvent)other);
					}
					base.MergeFrom(other);
					return this;
				}

				public override Builder MergeFrom(DepthMapEvent other)
				{
					if (other == DefaultInstance)
					{
						return this;
					}
					PrepareBuilder();
					if (other.HasTimestamp)
					{
						SetTimestamp(other.Timestamp);
					}
					if (other.HasWidth)
					{
						SetWidth(other.Width);
					}
					if (other.HasHeight)
					{
						SetHeight(other.Height);
					}
					if (other.zDistances_.Count != 0)
					{
						result.zDistances_.Add(other.zDistances_);
					}
					return this;
				}

				public override Builder MergeFrom(ICodedInputStream input)
				{
					return MergeFrom(input, ExtensionRegistry.Empty);
				}

				public override Builder MergeFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
				{
					PrepareBuilder();
					uint fieldTag;
					string fieldName;
					while (input.ReadTag(out fieldTag, out fieldName))
					{
						if (fieldTag == 0 && fieldName != null)
						{
							int num = Array.BinarySearch(_depthMapEventFieldNames, fieldName, StringComparer.Ordinal);
							if (num < 0)
							{
								ParseUnknownField(input, extensionRegistry, fieldTag, fieldName);
								continue;
							}
							fieldTag = _depthMapEventFieldTags[num];
						}
						switch (fieldTag)
						{
						case 0u:
							throw InvalidProtocolBufferException.InvalidTag();
						case 8u:
							result.hasTimestamp = input.ReadInt64(ref result.timestamp_);
							continue;
						case 16u:
							result.hasWidth = input.ReadInt32(ref result.width_);
							continue;
						case 24u:
							result.hasHeight = input.ReadInt32(ref result.height_);
							continue;
						case 34u:
						case 37u:
							input.ReadFloatArray(fieldTag, fieldName, result.zDistances_);
							continue;
						}
						if (WireFormat.IsEndGroupTag(fieldTag))
						{
							return this;
						}
						ParseUnknownField(input, extensionRegistry, fieldTag, fieldName);
					}
					return this;
				}

				public Builder SetTimestamp(long value)
				{
					PrepareBuilder();
					result.hasTimestamp = true;
					result.timestamp_ = value;
					return this;
				}

				public Builder ClearTimestamp()
				{
					PrepareBuilder();
					result.hasTimestamp = false;
					result.timestamp_ = 0L;
					return this;
				}

				public Builder SetWidth(int value)
				{
					PrepareBuilder();
					result.hasWidth = true;
					result.width_ = value;
					return this;
				}

				public Builder ClearWidth()
				{
					PrepareBuilder();
					result.hasWidth = false;
					result.width_ = 0;
					return this;
				}

				public Builder SetHeight(int value)
				{
					PrepareBuilder();
					result.hasHeight = true;
					result.height_ = value;
					return this;
				}

				public Builder ClearHeight()
				{
					PrepareBuilder();
					result.hasHeight = false;
					result.height_ = 0;
					return this;
				}

				public float GetZDistances(int index)
				{
					return result.GetZDistances(index);
				}

				public Builder SetZDistances(int index, float value)
				{
					PrepareBuilder();
					result.zDistances_[index] = value;
					return this;
				}

				public Builder AddZDistances(float value)
				{
					PrepareBuilder();
					result.zDistances_.Add(value);
					return this;
				}

				public Builder AddRangeZDistances(IEnumerable<float> values)
				{
					PrepareBuilder();
					result.zDistances_.Add(values);
					return this;
				}

				public Builder ClearZDistances()
				{
					PrepareBuilder();
					result.zDistances_.Clear();
					return this;
				}
			}

			private static readonly DepthMapEvent defaultInstance;

			private static readonly string[] _depthMapEventFieldNames;

			private static readonly uint[] _depthMapEventFieldTags;

			public const int TimestampFieldNumber = 1;

			private bool hasTimestamp;

			private long timestamp_;

			public const int WidthFieldNumber = 2;

			private bool hasWidth;

			private int width_;

			public const int HeightFieldNumber = 3;

			private bool hasHeight;

			private int height_;

			public const int ZDistancesFieldNumber = 4;

			private int zDistancesMemoizedSerializedSize;

			private PopsicleList<float> zDistances_ = new PopsicleList<float>();

			private int memoizedSerializedSize = -1;

			public static DepthMapEvent DefaultInstance => defaultInstance;

			public override DepthMapEvent DefaultInstanceForType => DefaultInstance;

			protected override DepthMapEvent ThisMessage => this;

			public bool HasTimestamp => hasTimestamp;

			public long Timestamp => timestamp_;

			public bool HasWidth => hasWidth;

			public int Width => width_;

			public bool HasHeight => hasHeight;

			public int Height => height_;

			public IList<float> ZDistancesList => Lists.AsReadOnly(zDistances_);

			public int ZDistancesCount => zDistances_.Count;

			public override bool IsInitialized => true;

			public override int SerializedSize
			{
				get
				{
					int num = memoizedSerializedSize;
					if (num != -1)
					{
						return num;
					}
					num = 0;
					if (hasTimestamp)
					{
						num += CodedOutputStream.ComputeInt64Size(1, Timestamp);
					}
					if (hasWidth)
					{
						num += CodedOutputStream.ComputeInt32Size(2, Width);
					}
					if (hasHeight)
					{
						num += CodedOutputStream.ComputeInt32Size(3, Height);
					}
					int num2 = 0;
					num2 = 4 * zDistances_.Count;
					num += num2;
					if (zDistances_.Count != 0)
					{
						num += 1 + CodedOutputStream.ComputeInt32SizeNoTag(num2);
					}
					zDistancesMemoizedSerializedSize = num2;
					memoizedSerializedSize = num;
					return num;
				}
			}

			private DepthMapEvent()
			{
			}

			static DepthMapEvent()
			{
				defaultInstance = new DepthMapEvent().MakeReadOnly();
				_depthMapEventFieldNames = new string[4] { "height", "timestamp", "width", "z_distances" };
				_depthMapEventFieldTags = new uint[4] { 24u, 8u, 16u, 34u };
				object.ReferenceEquals(proto.Proto.PhoneEvent.Descriptor, null);
			}

			public float GetZDistances(int index)
			{
				return zDistances_[index];
			}

			public override void WriteTo(ICodedOutputStream output)
			{
				string[] depthMapEventFieldNames = _depthMapEventFieldNames;
				if (hasTimestamp)
				{
					output.WriteInt64(1, depthMapEventFieldNames[1], Timestamp);
				}
				if (hasWidth)
				{
					output.WriteInt32(2, depthMapEventFieldNames[2], Width);
				}
				if (hasHeight)
				{
					output.WriteInt32(3, depthMapEventFieldNames[0], Height);
				}
				if (zDistances_.Count > 0)
				{
					output.WritePackedFloatArray(4, depthMapEventFieldNames[3], zDistancesMemoizedSerializedSize, zDistances_);
				}
			}

			public override int GetHashCode()
			{
				int num = GetType().GetHashCode();
				if (hasTimestamp)
				{
					num ^= timestamp_.GetHashCode();
				}
				if (hasWidth)
				{
					num ^= width_.GetHashCode();
				}
				if (hasHeight)
				{
					num ^= height_.GetHashCode();
				}
				foreach (float item in zDistances_)
				{
					num ^= item.GetHashCode();
				}
				return num;
			}

			public override bool Equals(object obj)
			{
				if (!(obj is DepthMapEvent depthMapEvent))
				{
					return false;
				}
				if (hasTimestamp != depthMapEvent.hasTimestamp || (hasTimestamp && !timestamp_.Equals(depthMapEvent.timestamp_)))
				{
					return false;
				}
				if (hasWidth != depthMapEvent.hasWidth || (hasWidth && !width_.Equals(depthMapEvent.width_)))
				{
					return false;
				}
				if (hasHeight != depthMapEvent.hasHeight || (hasHeight && !height_.Equals(depthMapEvent.height_)))
				{
					return false;
				}
				if (zDistances_.Count != depthMapEvent.zDistances_.Count)
				{
					return false;
				}
				for (int i = 0; i < zDistances_.Count; i++)
				{
					if (!zDistances_[i].Equals(depthMapEvent.zDistances_[i]))
					{
						return false;
					}
				}
				return true;
			}

			public override void PrintTo(TextWriter writer)
			{
				GeneratedMessageLite<DepthMapEvent, Builder>.PrintField("timestamp", hasTimestamp, timestamp_, writer);
				GeneratedMessageLite<DepthMapEvent, Builder>.PrintField("width", hasWidth, width_, writer);
				GeneratedMessageLite<DepthMapEvent, Builder>.PrintField("height", hasHeight, height_, writer);
				GeneratedMessageLite<DepthMapEvent, Builder>.PrintField("z_distances", zDistances_, writer);
			}

			public static DepthMapEvent ParseFrom(ByteString data)
			{
				return CreateBuilder().MergeFrom(data).BuildParsed();
			}

			public static DepthMapEvent ParseFrom(ByteString data, ExtensionRegistry extensionRegistry)
			{
				return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
			}

			public static DepthMapEvent ParseFrom(byte[] data)
			{
				return CreateBuilder().MergeFrom(data).BuildParsed();
			}

			public static DepthMapEvent ParseFrom(byte[] data, ExtensionRegistry extensionRegistry)
			{
				return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
			}

			public static DepthMapEvent ParseFrom(Stream input)
			{
				return CreateBuilder().MergeFrom(input).BuildParsed();
			}

			public static DepthMapEvent ParseFrom(Stream input, ExtensionRegistry extensionRegistry)
			{
				return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
			}

			public static DepthMapEvent ParseDelimitedFrom(Stream input)
			{
				return CreateBuilder().MergeDelimitedFrom(input).BuildParsed();
			}

			public static DepthMapEvent ParseDelimitedFrom(Stream input, ExtensionRegistry extensionRegistry)
			{
				return CreateBuilder().MergeDelimitedFrom(input, extensionRegistry).BuildParsed();
			}

			public static DepthMapEvent ParseFrom(ICodedInputStream input)
			{
				return CreateBuilder().MergeFrom(input).BuildParsed();
			}

			public static DepthMapEvent ParseFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
			{
				return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
			}

			private DepthMapEvent MakeReadOnly()
			{
				zDistances_.MakeReadOnly();
				return this;
			}

			public static Builder CreateBuilder()
			{
				return new Builder();
			}

			public override Builder ToBuilder()
			{
				return CreateBuilder(this);
			}

			public override Builder CreateBuilderForType()
			{
				return new Builder();
			}

			public static Builder CreateBuilder(DepthMapEvent prototype)
			{
				return new Builder(prototype);
			}
		}

		[DebuggerNonUserCode]
		[CompilerGenerated]
		[GeneratedCode("ProtoGen", "2.4.1.473")]
		public sealed class OrientationEvent : GeneratedMessageLite<OrientationEvent, OrientationEvent.Builder>
		{
			[DebuggerNonUserCode]
			[CompilerGenerated]
			[GeneratedCode("ProtoGen", "2.4.1.473")]
			public sealed class Builder : GeneratedBuilderLite<OrientationEvent, Builder>
			{
				private bool resultIsReadOnly;

				private OrientationEvent result;

				protected override Builder ThisBuilder => this;

				public override bool IsInitialized => result.IsInitialized;

				protected override OrientationEvent MessageBeingBuilt => PrepareBuilder();

				public override OrientationEvent DefaultInstanceForType => DefaultInstance;

				public bool HasTimestamp => result.hasTimestamp;

				public long Timestamp
				{
					get
					{
						return result.Timestamp;
					}
					set
					{
						SetTimestamp(value);
					}
				}

				public bool HasX => result.hasX;

				public float X
				{
					get
					{
						return result.X;
					}
					set
					{
						SetX(value);
					}
				}

				public bool HasY => result.hasY;

				public float Y
				{
					get
					{
						return result.Y;
					}
					set
					{
						SetY(value);
					}
				}

				public bool HasZ => result.hasZ;

				public float Z
				{
					get
					{
						return result.Z;
					}
					set
					{
						SetZ(value);
					}
				}

				public bool HasW => result.hasW;

				public float W
				{
					get
					{
						return result.W;
					}
					set
					{
						SetW(value);
					}
				}

				public Builder()
				{
					result = DefaultInstance;
					resultIsReadOnly = true;
				}

				internal Builder(OrientationEvent cloneFrom)
				{
					result = cloneFrom;
					resultIsReadOnly = true;
				}

				private OrientationEvent PrepareBuilder()
				{
					if (resultIsReadOnly)
					{
						OrientationEvent other = result;
						result = new OrientationEvent();
						resultIsReadOnly = false;
						MergeFrom(other);
					}
					return result;
				}

				public override Builder Clear()
				{
					result = DefaultInstance;
					resultIsReadOnly = true;
					return this;
				}

				public override Builder Clone()
				{
					if (resultIsReadOnly)
					{
						return new Builder(result);
					}
					return new Builder().MergeFrom(result);
				}

				public override OrientationEvent BuildPartial()
				{
					if (resultIsReadOnly)
					{
						return result;
					}
					resultIsReadOnly = true;
					return result.MakeReadOnly();
				}

				public override Builder MergeFrom(IMessageLite other)
				{
					if (other is OrientationEvent)
					{
						return MergeFrom((OrientationEvent)other);
					}
					base.MergeFrom(other);
					return this;
				}

				public override Builder MergeFrom(OrientationEvent other)
				{
					if (other == DefaultInstance)
					{
						return this;
					}
					PrepareBuilder();
					if (other.HasTimestamp)
					{
						SetTimestamp(other.Timestamp);
					}
					if (other.HasX)
					{
						SetX(other.X);
					}
					if (other.HasY)
					{
						SetY(other.Y);
					}
					if (other.HasZ)
					{
						SetZ(other.Z);
					}
					if (other.HasW)
					{
						SetW(other.W);
					}
					return this;
				}

				public override Builder MergeFrom(ICodedInputStream input)
				{
					return MergeFrom(input, ExtensionRegistry.Empty);
				}

				public override Builder MergeFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
				{
					PrepareBuilder();
					uint fieldTag;
					string fieldName;
					while (input.ReadTag(out fieldTag, out fieldName))
					{
						if (fieldTag == 0 && fieldName != null)
						{
							int num = Array.BinarySearch(_orientationEventFieldNames, fieldName, StringComparer.Ordinal);
							if (num < 0)
							{
								ParseUnknownField(input, extensionRegistry, fieldTag, fieldName);
								continue;
							}
							fieldTag = _orientationEventFieldTags[num];
						}
						switch (fieldTag)
						{
						case 0u:
							throw InvalidProtocolBufferException.InvalidTag();
						case 8u:
							result.hasTimestamp = input.ReadInt64(ref result.timestamp_);
							continue;
						case 21u:
							result.hasX = input.ReadFloat(ref result.x_);
							continue;
						case 29u:
							result.hasY = input.ReadFloat(ref result.y_);
							continue;
						case 37u:
							result.hasZ = input.ReadFloat(ref result.z_);
							continue;
						case 45u:
							result.hasW = input.ReadFloat(ref result.w_);
							continue;
						}
						if (WireFormat.IsEndGroupTag(fieldTag))
						{
							return this;
						}
						ParseUnknownField(input, extensionRegistry, fieldTag, fieldName);
					}
					return this;
				}

				public Builder SetTimestamp(long value)
				{
					PrepareBuilder();
					result.hasTimestamp = true;
					result.timestamp_ = value;
					return this;
				}

				public Builder ClearTimestamp()
				{
					PrepareBuilder();
					result.hasTimestamp = false;
					result.timestamp_ = 0L;
					return this;
				}

				public Builder SetX(float value)
				{
					PrepareBuilder();
					result.hasX = true;
					result.x_ = value;
					return this;
				}

				public Builder ClearX()
				{
					PrepareBuilder();
					result.hasX = false;
					result.x_ = 0f;
					return this;
				}

				public Builder SetY(float value)
				{
					PrepareBuilder();
					result.hasY = true;
					result.y_ = value;
					return this;
				}

				public Builder ClearY()
				{
					PrepareBuilder();
					result.hasY = false;
					result.y_ = 0f;
					return this;
				}

				public Builder SetZ(float value)
				{
					PrepareBuilder();
					result.hasZ = true;
					result.z_ = value;
					return this;
				}

				public Builder ClearZ()
				{
					PrepareBuilder();
					result.hasZ = false;
					result.z_ = 0f;
					return this;
				}

				public Builder SetW(float value)
				{
					PrepareBuilder();
					result.hasW = true;
					result.w_ = value;
					return this;
				}

				public Builder ClearW()
				{
					PrepareBuilder();
					result.hasW = false;
					result.w_ = 0f;
					return this;
				}
			}

			private static readonly OrientationEvent defaultInstance;

			private static readonly string[] _orientationEventFieldNames;

			private static readonly uint[] _orientationEventFieldTags;

			public const int TimestampFieldNumber = 1;

			private bool hasTimestamp;

			private long timestamp_;

			public const int XFieldNumber = 2;

			private bool hasX;

			private float x_;

			public const int YFieldNumber = 3;

			private bool hasY;

			private float y_;

			public const int ZFieldNumber = 4;

			private bool hasZ;

			private float z_;

			public const int WFieldNumber = 5;

			private bool hasW;

			private float w_;

			private int memoizedSerializedSize = -1;

			public static OrientationEvent DefaultInstance => defaultInstance;

			public override OrientationEvent DefaultInstanceForType => DefaultInstance;

			protected override OrientationEvent ThisMessage => this;

			public bool HasTimestamp => hasTimestamp;

			public long Timestamp => timestamp_;

			public bool HasX => hasX;

			public float X => x_;

			public bool HasY => hasY;

			public float Y => y_;

			public bool HasZ => hasZ;

			public float Z => z_;

			public bool HasW => hasW;

			public float W => w_;

			public override bool IsInitialized => true;

			public override int SerializedSize
			{
				get
				{
					int num = memoizedSerializedSize;
					if (num != -1)
					{
						return num;
					}
					num = 0;
					if (hasTimestamp)
					{
						num += CodedOutputStream.ComputeInt64Size(1, Timestamp);
					}
					if (hasX)
					{
						num += CodedOutputStream.ComputeFloatSize(2, X);
					}
					if (hasY)
					{
						num += CodedOutputStream.ComputeFloatSize(3, Y);
					}
					if (hasZ)
					{
						num += CodedOutputStream.ComputeFloatSize(4, Z);
					}
					if (hasW)
					{
						num += CodedOutputStream.ComputeFloatSize(5, W);
					}
					memoizedSerializedSize = num;
					return num;
				}
			}

			private OrientationEvent()
			{
			}

			static OrientationEvent()
			{
				defaultInstance = new OrientationEvent().MakeReadOnly();
				_orientationEventFieldNames = new string[5] { "timestamp", "w", "x", "y", "z" };
				_orientationEventFieldTags = new uint[5] { 8u, 45u, 21u, 29u, 37u };
				object.ReferenceEquals(proto.Proto.PhoneEvent.Descriptor, null);
			}

			public override void WriteTo(ICodedOutputStream output)
			{
				string[] orientationEventFieldNames = _orientationEventFieldNames;
				if (hasTimestamp)
				{
					output.WriteInt64(1, orientationEventFieldNames[0], Timestamp);
				}
				if (hasX)
				{
					output.WriteFloat(2, orientationEventFieldNames[2], X);
				}
				if (hasY)
				{
					output.WriteFloat(3, orientationEventFieldNames[3], Y);
				}
				if (hasZ)
				{
					output.WriteFloat(4, orientationEventFieldNames[4], Z);
				}
				if (hasW)
				{
					output.WriteFloat(5, orientationEventFieldNames[1], W);
				}
			}

			public override int GetHashCode()
			{
				int num = GetType().GetHashCode();
				if (hasTimestamp)
				{
					num ^= timestamp_.GetHashCode();
				}
				if (hasX)
				{
					num ^= x_.GetHashCode();
				}
				if (hasY)
				{
					num ^= y_.GetHashCode();
				}
				if (hasZ)
				{
					num ^= z_.GetHashCode();
				}
				if (hasW)
				{
					num ^= w_.GetHashCode();
				}
				return num;
			}

			public override bool Equals(object obj)
			{
				if (!(obj is OrientationEvent orientationEvent))
				{
					return false;
				}
				if (hasTimestamp != orientationEvent.hasTimestamp || (hasTimestamp && !timestamp_.Equals(orientationEvent.timestamp_)))
				{
					return false;
				}
				if (hasX != orientationEvent.hasX || (hasX && !x_.Equals(orientationEvent.x_)))
				{
					return false;
				}
				if (hasY != orientationEvent.hasY || (hasY && !y_.Equals(orientationEvent.y_)))
				{
					return false;
				}
				if (hasZ != orientationEvent.hasZ || (hasZ && !z_.Equals(orientationEvent.z_)))
				{
					return false;
				}
				if (hasW != orientationEvent.hasW || (hasW && !w_.Equals(orientationEvent.w_)))
				{
					return false;
				}
				return true;
			}

			public override void PrintTo(TextWriter writer)
			{
				GeneratedMessageLite<OrientationEvent, Builder>.PrintField("timestamp", hasTimestamp, timestamp_, writer);
				GeneratedMessageLite<OrientationEvent, Builder>.PrintField("x", hasX, x_, writer);
				GeneratedMessageLite<OrientationEvent, Builder>.PrintField("y", hasY, y_, writer);
				GeneratedMessageLite<OrientationEvent, Builder>.PrintField("z", hasZ, z_, writer);
				GeneratedMessageLite<OrientationEvent, Builder>.PrintField("w", hasW, w_, writer);
			}

			public static OrientationEvent ParseFrom(ByteString data)
			{
				return CreateBuilder().MergeFrom(data).BuildParsed();
			}

			public static OrientationEvent ParseFrom(ByteString data, ExtensionRegistry extensionRegistry)
			{
				return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
			}

			public static OrientationEvent ParseFrom(byte[] data)
			{
				return CreateBuilder().MergeFrom(data).BuildParsed();
			}

			public static OrientationEvent ParseFrom(byte[] data, ExtensionRegistry extensionRegistry)
			{
				return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
			}

			public static OrientationEvent ParseFrom(Stream input)
			{
				return CreateBuilder().MergeFrom(input).BuildParsed();
			}

			public static OrientationEvent ParseFrom(Stream input, ExtensionRegistry extensionRegistry)
			{
				return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
			}

			public static OrientationEvent ParseDelimitedFrom(Stream input)
			{
				return CreateBuilder().MergeDelimitedFrom(input).BuildParsed();
			}

			public static OrientationEvent ParseDelimitedFrom(Stream input, ExtensionRegistry extensionRegistry)
			{
				return CreateBuilder().MergeDelimitedFrom(input, extensionRegistry).BuildParsed();
			}

			public static OrientationEvent ParseFrom(ICodedInputStream input)
			{
				return CreateBuilder().MergeFrom(input).BuildParsed();
			}

			public static OrientationEvent ParseFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
			{
				return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
			}

			private OrientationEvent MakeReadOnly()
			{
				return this;
			}

			public static Builder CreateBuilder()
			{
				return new Builder();
			}

			public override Builder ToBuilder()
			{
				return CreateBuilder(this);
			}

			public override Builder CreateBuilderForType()
			{
				return new Builder();
			}

			public static Builder CreateBuilder(OrientationEvent prototype)
			{
				return new Builder(prototype);
			}
		}

		[DebuggerNonUserCode]
		[CompilerGenerated]
		[GeneratedCode("ProtoGen", "2.4.1.473")]
		public sealed class KeyEvent : GeneratedMessageLite<KeyEvent, KeyEvent.Builder>
		{
			[DebuggerNonUserCode]
			[CompilerGenerated]
			[GeneratedCode("ProtoGen", "2.4.1.473")]
			public sealed class Builder : GeneratedBuilderLite<KeyEvent, Builder>
			{
				private bool resultIsReadOnly;

				private KeyEvent result;

				protected override Builder ThisBuilder => this;

				public override bool IsInitialized => result.IsInitialized;

				protected override KeyEvent MessageBeingBuilt => PrepareBuilder();

				public override KeyEvent DefaultInstanceForType => DefaultInstance;

				public bool HasAction => result.hasAction;

				public int Action
				{
					get
					{
						return result.Action;
					}
					set
					{
						SetAction(value);
					}
				}

				public bool HasCode => result.hasCode;

				public int Code
				{
					get
					{
						return result.Code;
					}
					set
					{
						SetCode(value);
					}
				}

				public Builder()
				{
					result = DefaultInstance;
					resultIsReadOnly = true;
				}

				internal Builder(KeyEvent cloneFrom)
				{
					result = cloneFrom;
					resultIsReadOnly = true;
				}

				private KeyEvent PrepareBuilder()
				{
					if (resultIsReadOnly)
					{
						KeyEvent other = result;
						result = new KeyEvent();
						resultIsReadOnly = false;
						MergeFrom(other);
					}
					return result;
				}

				public override Builder Clear()
				{
					result = DefaultInstance;
					resultIsReadOnly = true;
					return this;
				}

				public override Builder Clone()
				{
					if (resultIsReadOnly)
					{
						return new Builder(result);
					}
					return new Builder().MergeFrom(result);
				}

				public override KeyEvent BuildPartial()
				{
					if (resultIsReadOnly)
					{
						return result;
					}
					resultIsReadOnly = true;
					return result.MakeReadOnly();
				}

				public override Builder MergeFrom(IMessageLite other)
				{
					if (other is KeyEvent)
					{
						return MergeFrom((KeyEvent)other);
					}
					base.MergeFrom(other);
					return this;
				}

				public override Builder MergeFrom(KeyEvent other)
				{
					if (other == DefaultInstance)
					{
						return this;
					}
					PrepareBuilder();
					if (other.HasAction)
					{
						SetAction(other.Action);
					}
					if (other.HasCode)
					{
						SetCode(other.Code);
					}
					return this;
				}

				public override Builder MergeFrom(ICodedInputStream input)
				{
					return MergeFrom(input, ExtensionRegistry.Empty);
				}

				public override Builder MergeFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
				{
					PrepareBuilder();
					uint fieldTag;
					string fieldName;
					while (input.ReadTag(out fieldTag, out fieldName))
					{
						if (fieldTag == 0 && fieldName != null)
						{
							int num = Array.BinarySearch(_keyEventFieldNames, fieldName, StringComparer.Ordinal);
							if (num < 0)
							{
								ParseUnknownField(input, extensionRegistry, fieldTag, fieldName);
								continue;
							}
							fieldTag = _keyEventFieldTags[num];
						}
						switch (fieldTag)
						{
						case 0u:
							throw InvalidProtocolBufferException.InvalidTag();
						case 8u:
							result.hasAction = input.ReadInt32(ref result.action_);
							continue;
						case 16u:
							result.hasCode = input.ReadInt32(ref result.code_);
							continue;
						}
						if (WireFormat.IsEndGroupTag(fieldTag))
						{
							return this;
						}
						ParseUnknownField(input, extensionRegistry, fieldTag, fieldName);
					}
					return this;
				}

				public Builder SetAction(int value)
				{
					PrepareBuilder();
					result.hasAction = true;
					result.action_ = value;
					return this;
				}

				public Builder ClearAction()
				{
					PrepareBuilder();
					result.hasAction = false;
					result.action_ = 0;
					return this;
				}

				public Builder SetCode(int value)
				{
					PrepareBuilder();
					result.hasCode = true;
					result.code_ = value;
					return this;
				}

				public Builder ClearCode()
				{
					PrepareBuilder();
					result.hasCode = false;
					result.code_ = 0;
					return this;
				}
			}

			private static readonly KeyEvent defaultInstance;

			private static readonly string[] _keyEventFieldNames;

			private static readonly uint[] _keyEventFieldTags;

			public const int ActionFieldNumber = 1;

			private bool hasAction;

			private int action_;

			public const int CodeFieldNumber = 2;

			private bool hasCode;

			private int code_;

			private int memoizedSerializedSize = -1;

			public static KeyEvent DefaultInstance => defaultInstance;

			public override KeyEvent DefaultInstanceForType => DefaultInstance;

			protected override KeyEvent ThisMessage => this;

			public bool HasAction => hasAction;

			public int Action => action_;

			public bool HasCode => hasCode;

			public int Code => code_;

			public override bool IsInitialized => true;

			public override int SerializedSize
			{
				get
				{
					int num = memoizedSerializedSize;
					if (num != -1)
					{
						return num;
					}
					num = 0;
					if (hasAction)
					{
						num += CodedOutputStream.ComputeInt32Size(1, Action);
					}
					if (hasCode)
					{
						num += CodedOutputStream.ComputeInt32Size(2, Code);
					}
					memoizedSerializedSize = num;
					return num;
				}
			}

			private KeyEvent()
			{
			}

			static KeyEvent()
			{
				defaultInstance = new KeyEvent().MakeReadOnly();
				_keyEventFieldNames = new string[2] { "action", "code" };
				_keyEventFieldTags = new uint[2] { 8u, 16u };
				object.ReferenceEquals(proto.Proto.PhoneEvent.Descriptor, null);
			}

			public override void WriteTo(ICodedOutputStream output)
			{
				string[] keyEventFieldNames = _keyEventFieldNames;
				if (hasAction)
				{
					output.WriteInt32(1, keyEventFieldNames[0], Action);
				}
				if (hasCode)
				{
					output.WriteInt32(2, keyEventFieldNames[1], Code);
				}
			}

			public override int GetHashCode()
			{
				int num = GetType().GetHashCode();
				if (hasAction)
				{
					num ^= action_.GetHashCode();
				}
				if (hasCode)
				{
					num ^= code_.GetHashCode();
				}
				return num;
			}

			public override bool Equals(object obj)
			{
				if (!(obj is KeyEvent keyEvent))
				{
					return false;
				}
				if (hasAction != keyEvent.hasAction || (hasAction && !action_.Equals(keyEvent.action_)))
				{
					return false;
				}
				if (hasCode != keyEvent.hasCode || (hasCode && !code_.Equals(keyEvent.code_)))
				{
					return false;
				}
				return true;
			}

			public override void PrintTo(TextWriter writer)
			{
				GeneratedMessageLite<KeyEvent, Builder>.PrintField("action", hasAction, action_, writer);
				GeneratedMessageLite<KeyEvent, Builder>.PrintField("code", hasCode, code_, writer);
			}

			public static KeyEvent ParseFrom(ByteString data)
			{
				return CreateBuilder().MergeFrom(data).BuildParsed();
			}

			public static KeyEvent ParseFrom(ByteString data, ExtensionRegistry extensionRegistry)
			{
				return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
			}

			public static KeyEvent ParseFrom(byte[] data)
			{
				return CreateBuilder().MergeFrom(data).BuildParsed();
			}

			public static KeyEvent ParseFrom(byte[] data, ExtensionRegistry extensionRegistry)
			{
				return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
			}

			public static KeyEvent ParseFrom(Stream input)
			{
				return CreateBuilder().MergeFrom(input).BuildParsed();
			}

			public static KeyEvent ParseFrom(Stream input, ExtensionRegistry extensionRegistry)
			{
				return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
			}

			public static KeyEvent ParseDelimitedFrom(Stream input)
			{
				return CreateBuilder().MergeDelimitedFrom(input).BuildParsed();
			}

			public static KeyEvent ParseDelimitedFrom(Stream input, ExtensionRegistry extensionRegistry)
			{
				return CreateBuilder().MergeDelimitedFrom(input, extensionRegistry).BuildParsed();
			}

			public static KeyEvent ParseFrom(ICodedInputStream input)
			{
				return CreateBuilder().MergeFrom(input).BuildParsed();
			}

			public static KeyEvent ParseFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
			{
				return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
			}

			private KeyEvent MakeReadOnly()
			{
				return this;
			}

			public static Builder CreateBuilder()
			{
				return new Builder();
			}

			public override Builder ToBuilder()
			{
				return CreateBuilder(this);
			}

			public override Builder CreateBuilderForType()
			{
				return new Builder();
			}

			public static Builder CreateBuilder(KeyEvent prototype)
			{
				return new Builder(prototype);
			}
		}
	}

	[DebuggerNonUserCode]
	[CompilerGenerated]
	[GeneratedCode("ProtoGen", "2.4.1.473")]
	public sealed class Builder : GeneratedBuilderLite<PhoneEvent, Builder>
	{
		private bool resultIsReadOnly;

		private PhoneEvent result;

		protected override Builder ThisBuilder => this;

		public override bool IsInitialized => result.IsInitialized;

		protected override PhoneEvent MessageBeingBuilt => PrepareBuilder();

		public override PhoneEvent DefaultInstanceForType => DefaultInstance;

		public bool HasType => result.hasType;

		public Types.Type Type
		{
			get
			{
				return result.Type;
			}
			set
			{
				SetType(value);
			}
		}

		public bool HasMotionEvent => result.hasMotionEvent;

		public Types.MotionEvent MotionEvent
		{
			get
			{
				return result.MotionEvent;
			}
			set
			{
				SetMotionEvent(value);
			}
		}

		public bool HasGyroscopeEvent => result.hasGyroscopeEvent;

		public Types.GyroscopeEvent GyroscopeEvent
		{
			get
			{
				return result.GyroscopeEvent;
			}
			set
			{
				SetGyroscopeEvent(value);
			}
		}

		public bool HasAccelerometerEvent => result.hasAccelerometerEvent;

		public Types.AccelerometerEvent AccelerometerEvent
		{
			get
			{
				return result.AccelerometerEvent;
			}
			set
			{
				SetAccelerometerEvent(value);
			}
		}

		public bool HasDepthMapEvent => result.hasDepthMapEvent;

		public Types.DepthMapEvent DepthMapEvent
		{
			get
			{
				return result.DepthMapEvent;
			}
			set
			{
				SetDepthMapEvent(value);
			}
		}

		public bool HasOrientationEvent => result.hasOrientationEvent;

		public Types.OrientationEvent OrientationEvent
		{
			get
			{
				return result.OrientationEvent;
			}
			set
			{
				SetOrientationEvent(value);
			}
		}

		public bool HasKeyEvent => result.hasKeyEvent;

		public Types.KeyEvent KeyEvent
		{
			get
			{
				return result.KeyEvent;
			}
			set
			{
				SetKeyEvent(value);
			}
		}

		public Builder()
		{
			result = DefaultInstance;
			resultIsReadOnly = true;
		}

		internal Builder(PhoneEvent cloneFrom)
		{
			result = cloneFrom;
			resultIsReadOnly = true;
		}

		private PhoneEvent PrepareBuilder()
		{
			if (resultIsReadOnly)
			{
				PhoneEvent other = result;
				result = new PhoneEvent();
				resultIsReadOnly = false;
				MergeFrom(other);
			}
			return result;
		}

		public override Builder Clear()
		{
			result = DefaultInstance;
			resultIsReadOnly = true;
			return this;
		}

		public override Builder Clone()
		{
			if (resultIsReadOnly)
			{
				return new Builder(result);
			}
			return new Builder().MergeFrom(result);
		}

		public override PhoneEvent BuildPartial()
		{
			if (resultIsReadOnly)
			{
				return result;
			}
			resultIsReadOnly = true;
			return result.MakeReadOnly();
		}

		public override Builder MergeFrom(IMessageLite other)
		{
			if (other is PhoneEvent)
			{
				return MergeFrom((PhoneEvent)other);
			}
			base.MergeFrom(other);
			return this;
		}

		public override Builder MergeFrom(PhoneEvent other)
		{
			if (other == DefaultInstance)
			{
				return this;
			}
			PrepareBuilder();
			if (other.HasType)
			{
				SetType(other.Type);
			}
			if (other.HasMotionEvent)
			{
				MergeMotionEvent(other.MotionEvent);
			}
			if (other.HasGyroscopeEvent)
			{
				MergeGyroscopeEvent(other.GyroscopeEvent);
			}
			if (other.HasAccelerometerEvent)
			{
				MergeAccelerometerEvent(other.AccelerometerEvent);
			}
			if (other.HasDepthMapEvent)
			{
				MergeDepthMapEvent(other.DepthMapEvent);
			}
			if (other.HasOrientationEvent)
			{
				MergeOrientationEvent(other.OrientationEvent);
			}
			if (other.HasKeyEvent)
			{
				MergeKeyEvent(other.KeyEvent);
			}
			return this;
		}

		public override Builder MergeFrom(ICodedInputStream input)
		{
			return MergeFrom(input, ExtensionRegistry.Empty);
		}

		public override Builder MergeFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
		{
			PrepareBuilder();
			uint fieldTag;
			string fieldName;
			while (input.ReadTag(out fieldTag, out fieldName))
			{
				if (fieldTag == 0 && fieldName != null)
				{
					int num = Array.BinarySearch(_phoneEventFieldNames, fieldName, StringComparer.Ordinal);
					if (num < 0)
					{
						ParseUnknownField(input, extensionRegistry, fieldTag, fieldName);
						continue;
					}
					fieldTag = _phoneEventFieldTags[num];
				}
				switch (fieldTag)
				{
				case 0u:
					throw InvalidProtocolBufferException.InvalidTag();
				default:
					if (WireFormat.IsEndGroupTag(fieldTag))
					{
						return this;
					}
					ParseUnknownField(input, extensionRegistry, fieldTag, fieldName);
					break;
				case 8u:
				{
					if (input.ReadEnum(ref result.type_, out var unknown))
					{
						result.hasType = true;
					}
					else if (!(unknown is int))
					{
					}
					break;
				}
				case 18u:
				{
					Types.MotionEvent.Builder builder5 = Types.MotionEvent.CreateBuilder();
					if (result.hasMotionEvent)
					{
						builder5.MergeFrom(MotionEvent);
					}
					input.ReadMessage(builder5, extensionRegistry);
					SetMotionEvent(builder5.BuildPartial());
					break;
				}
				case 26u:
				{
					Types.GyroscopeEvent.Builder builder4 = Types.GyroscopeEvent.CreateBuilder();
					if (result.hasGyroscopeEvent)
					{
						builder4.MergeFrom(GyroscopeEvent);
					}
					input.ReadMessage(builder4, extensionRegistry);
					SetGyroscopeEvent(builder4.BuildPartial());
					break;
				}
				case 34u:
				{
					Types.AccelerometerEvent.Builder builder2 = Types.AccelerometerEvent.CreateBuilder();
					if (result.hasAccelerometerEvent)
					{
						builder2.MergeFrom(AccelerometerEvent);
					}
					input.ReadMessage(builder2, extensionRegistry);
					SetAccelerometerEvent(builder2.BuildPartial());
					break;
				}
				case 42u:
				{
					Types.DepthMapEvent.Builder builder6 = Types.DepthMapEvent.CreateBuilder();
					if (result.hasDepthMapEvent)
					{
						builder6.MergeFrom(DepthMapEvent);
					}
					input.ReadMessage(builder6, extensionRegistry);
					SetDepthMapEvent(builder6.BuildPartial());
					break;
				}
				case 50u:
				{
					Types.OrientationEvent.Builder builder3 = Types.OrientationEvent.CreateBuilder();
					if (result.hasOrientationEvent)
					{
						builder3.MergeFrom(OrientationEvent);
					}
					input.ReadMessage(builder3, extensionRegistry);
					SetOrientationEvent(builder3.BuildPartial());
					break;
				}
				case 58u:
				{
					Types.KeyEvent.Builder builder = Types.KeyEvent.CreateBuilder();
					if (result.hasKeyEvent)
					{
						builder.MergeFrom(KeyEvent);
					}
					input.ReadMessage(builder, extensionRegistry);
					SetKeyEvent(builder.BuildPartial());
					break;
				}
				}
			}
			return this;
		}

		public Builder SetType(Types.Type value)
		{
			PrepareBuilder();
			result.hasType = true;
			result.type_ = value;
			return this;
		}

		public Builder ClearType()
		{
			PrepareBuilder();
			result.hasType = false;
			result.type_ = Types.Type.MOTION;
			return this;
		}

		public Builder SetMotionEvent(Types.MotionEvent value)
		{
			ThrowHelper.ThrowIfNull(value, "value");
			PrepareBuilder();
			result.hasMotionEvent = true;
			result.motionEvent_ = value;
			return this;
		}

		public Builder SetMotionEvent(Types.MotionEvent.Builder builderForValue)
		{
			ThrowHelper.ThrowIfNull(builderForValue, "builderForValue");
			PrepareBuilder();
			result.hasMotionEvent = true;
			result.motionEvent_ = builderForValue.Build();
			return this;
		}

		public Builder MergeMotionEvent(Types.MotionEvent value)
		{
			ThrowHelper.ThrowIfNull(value, "value");
			PrepareBuilder();
			if (result.hasMotionEvent && result.motionEvent_ != Types.MotionEvent.DefaultInstance)
			{
				result.motionEvent_ = Types.MotionEvent.CreateBuilder(result.motionEvent_).MergeFrom(value).BuildPartial();
			}
			else
			{
				result.motionEvent_ = value;
			}
			result.hasMotionEvent = true;
			return this;
		}

		public Builder ClearMotionEvent()
		{
			PrepareBuilder();
			result.hasMotionEvent = false;
			result.motionEvent_ = null;
			return this;
		}

		public Builder SetGyroscopeEvent(Types.GyroscopeEvent value)
		{
			ThrowHelper.ThrowIfNull(value, "value");
			PrepareBuilder();
			result.hasGyroscopeEvent = true;
			result.gyroscopeEvent_ = value;
			return this;
		}

		public Builder SetGyroscopeEvent(Types.GyroscopeEvent.Builder builderForValue)
		{
			ThrowHelper.ThrowIfNull(builderForValue, "builderForValue");
			PrepareBuilder();
			result.hasGyroscopeEvent = true;
			result.gyroscopeEvent_ = builderForValue.Build();
			return this;
		}

		public Builder MergeGyroscopeEvent(Types.GyroscopeEvent value)
		{
			ThrowHelper.ThrowIfNull(value, "value");
			PrepareBuilder();
			if (result.hasGyroscopeEvent && result.gyroscopeEvent_ != Types.GyroscopeEvent.DefaultInstance)
			{
				result.gyroscopeEvent_ = Types.GyroscopeEvent.CreateBuilder(result.gyroscopeEvent_).MergeFrom(value).BuildPartial();
			}
			else
			{
				result.gyroscopeEvent_ = value;
			}
			result.hasGyroscopeEvent = true;
			return this;
		}

		public Builder ClearGyroscopeEvent()
		{
			PrepareBuilder();
			result.hasGyroscopeEvent = false;
			result.gyroscopeEvent_ = null;
			return this;
		}

		public Builder SetAccelerometerEvent(Types.AccelerometerEvent value)
		{
			ThrowHelper.ThrowIfNull(value, "value");
			PrepareBuilder();
			result.hasAccelerometerEvent = true;
			result.accelerometerEvent_ = value;
			return this;
		}

		public Builder SetAccelerometerEvent(Types.AccelerometerEvent.Builder builderForValue)
		{
			ThrowHelper.ThrowIfNull(builderForValue, "builderForValue");
			PrepareBuilder();
			result.hasAccelerometerEvent = true;
			result.accelerometerEvent_ = builderForValue.Build();
			return this;
		}

		public Builder MergeAccelerometerEvent(Types.AccelerometerEvent value)
		{
			ThrowHelper.ThrowIfNull(value, "value");
			PrepareBuilder();
			if (result.hasAccelerometerEvent && result.accelerometerEvent_ != Types.AccelerometerEvent.DefaultInstance)
			{
				result.accelerometerEvent_ = Types.AccelerometerEvent.CreateBuilder(result.accelerometerEvent_).MergeFrom(value).BuildPartial();
			}
			else
			{
				result.accelerometerEvent_ = value;
			}
			result.hasAccelerometerEvent = true;
			return this;
		}

		public Builder ClearAccelerometerEvent()
		{
			PrepareBuilder();
			result.hasAccelerometerEvent = false;
			result.accelerometerEvent_ = null;
			return this;
		}

		public Builder SetDepthMapEvent(Types.DepthMapEvent value)
		{
			ThrowHelper.ThrowIfNull(value, "value");
			PrepareBuilder();
			result.hasDepthMapEvent = true;
			result.depthMapEvent_ = value;
			return this;
		}

		public Builder SetDepthMapEvent(Types.DepthMapEvent.Builder builderForValue)
		{
			ThrowHelper.ThrowIfNull(builderForValue, "builderForValue");
			PrepareBuilder();
			result.hasDepthMapEvent = true;
			result.depthMapEvent_ = builderForValue.Build();
			return this;
		}

		public Builder MergeDepthMapEvent(Types.DepthMapEvent value)
		{
			ThrowHelper.ThrowIfNull(value, "value");
			PrepareBuilder();
			if (result.hasDepthMapEvent && result.depthMapEvent_ != Types.DepthMapEvent.DefaultInstance)
			{
				result.depthMapEvent_ = Types.DepthMapEvent.CreateBuilder(result.depthMapEvent_).MergeFrom(value).BuildPartial();
			}
			else
			{
				result.depthMapEvent_ = value;
			}
			result.hasDepthMapEvent = true;
			return this;
		}

		public Builder ClearDepthMapEvent()
		{
			PrepareBuilder();
			result.hasDepthMapEvent = false;
			result.depthMapEvent_ = null;
			return this;
		}

		public Builder SetOrientationEvent(Types.OrientationEvent value)
		{
			ThrowHelper.ThrowIfNull(value, "value");
			PrepareBuilder();
			result.hasOrientationEvent = true;
			result.orientationEvent_ = value;
			return this;
		}

		public Builder SetOrientationEvent(Types.OrientationEvent.Builder builderForValue)
		{
			ThrowHelper.ThrowIfNull(builderForValue, "builderForValue");
			PrepareBuilder();
			result.hasOrientationEvent = true;
			result.orientationEvent_ = builderForValue.Build();
			return this;
		}

		public Builder MergeOrientationEvent(Types.OrientationEvent value)
		{
			ThrowHelper.ThrowIfNull(value, "value");
			PrepareBuilder();
			if (result.hasOrientationEvent && result.orientationEvent_ != Types.OrientationEvent.DefaultInstance)
			{
				result.orientationEvent_ = Types.OrientationEvent.CreateBuilder(result.orientationEvent_).MergeFrom(value).BuildPartial();
			}
			else
			{
				result.orientationEvent_ = value;
			}
			result.hasOrientationEvent = true;
			return this;
		}

		public Builder ClearOrientationEvent()
		{
			PrepareBuilder();
			result.hasOrientationEvent = false;
			result.orientationEvent_ = null;
			return this;
		}

		public Builder SetKeyEvent(Types.KeyEvent value)
		{
			ThrowHelper.ThrowIfNull(value, "value");
			PrepareBuilder();
			result.hasKeyEvent = true;
			result.keyEvent_ = value;
			return this;
		}

		public Builder SetKeyEvent(Types.KeyEvent.Builder builderForValue)
		{
			ThrowHelper.ThrowIfNull(builderForValue, "builderForValue");
			PrepareBuilder();
			result.hasKeyEvent = true;
			result.keyEvent_ = builderForValue.Build();
			return this;
		}

		public Builder MergeKeyEvent(Types.KeyEvent value)
		{
			ThrowHelper.ThrowIfNull(value, "value");
			PrepareBuilder();
			if (result.hasKeyEvent && result.keyEvent_ != Types.KeyEvent.DefaultInstance)
			{
				result.keyEvent_ = Types.KeyEvent.CreateBuilder(result.keyEvent_).MergeFrom(value).BuildPartial();
			}
			else
			{
				result.keyEvent_ = value;
			}
			result.hasKeyEvent = true;
			return this;
		}

		public Builder ClearKeyEvent()
		{
			PrepareBuilder();
			result.hasKeyEvent = false;
			result.keyEvent_ = null;
			return this;
		}
	}

	private static readonly PhoneEvent defaultInstance;

	private static readonly string[] _phoneEventFieldNames;

	private static readonly uint[] _phoneEventFieldTags;

	public const int TypeFieldNumber = 1;

	private bool hasType;

	private Types.Type type_ = Types.Type.MOTION;

	public const int MotionEventFieldNumber = 2;

	private bool hasMotionEvent;

	private Types.MotionEvent motionEvent_;

	public const int GyroscopeEventFieldNumber = 3;

	private bool hasGyroscopeEvent;

	private Types.GyroscopeEvent gyroscopeEvent_;

	public const int AccelerometerEventFieldNumber = 4;

	private bool hasAccelerometerEvent;

	private Types.AccelerometerEvent accelerometerEvent_;

	public const int DepthMapEventFieldNumber = 5;

	private bool hasDepthMapEvent;

	private Types.DepthMapEvent depthMapEvent_;

	public const int OrientationEventFieldNumber = 6;

	private bool hasOrientationEvent;

	private Types.OrientationEvent orientationEvent_;

	public const int KeyEventFieldNumber = 7;

	private bool hasKeyEvent;

	private Types.KeyEvent keyEvent_;

	private int memoizedSerializedSize = -1;

	public static PhoneEvent DefaultInstance => defaultInstance;

	public override PhoneEvent DefaultInstanceForType => DefaultInstance;

	protected override PhoneEvent ThisMessage => this;

	public bool HasType => hasType;

	public Types.Type Type => type_;

	public bool HasMotionEvent => hasMotionEvent;

	public Types.MotionEvent MotionEvent => motionEvent_ ?? Types.MotionEvent.DefaultInstance;

	public bool HasGyroscopeEvent => hasGyroscopeEvent;

	public Types.GyroscopeEvent GyroscopeEvent => gyroscopeEvent_ ?? Types.GyroscopeEvent.DefaultInstance;

	public bool HasAccelerometerEvent => hasAccelerometerEvent;

	public Types.AccelerometerEvent AccelerometerEvent => accelerometerEvent_ ?? Types.AccelerometerEvent.DefaultInstance;

	public bool HasDepthMapEvent => hasDepthMapEvent;

	public Types.DepthMapEvent DepthMapEvent => depthMapEvent_ ?? Types.DepthMapEvent.DefaultInstance;

	public bool HasOrientationEvent => hasOrientationEvent;

	public Types.OrientationEvent OrientationEvent => orientationEvent_ ?? Types.OrientationEvent.DefaultInstance;

	public bool HasKeyEvent => hasKeyEvent;

	public Types.KeyEvent KeyEvent => keyEvent_ ?? Types.KeyEvent.DefaultInstance;

	public override bool IsInitialized => true;

	public override int SerializedSize
	{
		get
		{
			int num = memoizedSerializedSize;
			if (num != -1)
			{
				return num;
			}
			num = 0;
			if (hasType)
			{
				num += CodedOutputStream.ComputeEnumSize(1, (int)Type);
			}
			if (hasMotionEvent)
			{
				num += CodedOutputStream.ComputeMessageSize(2, MotionEvent);
			}
			if (hasGyroscopeEvent)
			{
				num += CodedOutputStream.ComputeMessageSize(3, GyroscopeEvent);
			}
			if (hasAccelerometerEvent)
			{
				num += CodedOutputStream.ComputeMessageSize(4, AccelerometerEvent);
			}
			if (hasDepthMapEvent)
			{
				num += CodedOutputStream.ComputeMessageSize(5, DepthMapEvent);
			}
			if (hasOrientationEvent)
			{
				num += CodedOutputStream.ComputeMessageSize(6, OrientationEvent);
			}
			if (hasKeyEvent)
			{
				num += CodedOutputStream.ComputeMessageSize(7, KeyEvent);
			}
			memoizedSerializedSize = num;
			return num;
		}
	}

	private PhoneEvent()
	{
	}

	static PhoneEvent()
	{
		defaultInstance = new PhoneEvent().MakeReadOnly();
		_phoneEventFieldNames = new string[7] { "accelerometer_event", "depth_map_event", "gyroscope_event", "key_event", "motion_event", "orientation_event", "type" };
		_phoneEventFieldTags = new uint[7] { 34u, 42u, 26u, 58u, 18u, 50u, 8u };
		object.ReferenceEquals(proto.Proto.PhoneEvent.Descriptor, null);
	}

	public override void WriteTo(ICodedOutputStream output)
	{
		string[] phoneEventFieldNames = _phoneEventFieldNames;
		if (hasType)
		{
			output.WriteEnum(1, phoneEventFieldNames[6], (int)Type, Type);
		}
		if (hasMotionEvent)
		{
			output.WriteMessage(2, phoneEventFieldNames[4], MotionEvent);
		}
		if (hasGyroscopeEvent)
		{
			output.WriteMessage(3, phoneEventFieldNames[2], GyroscopeEvent);
		}
		if (hasAccelerometerEvent)
		{
			output.WriteMessage(4, phoneEventFieldNames[0], AccelerometerEvent);
		}
		if (hasDepthMapEvent)
		{
			output.WriteMessage(5, phoneEventFieldNames[1], DepthMapEvent);
		}
		if (hasOrientationEvent)
		{
			output.WriteMessage(6, phoneEventFieldNames[5], OrientationEvent);
		}
		if (hasKeyEvent)
		{
			output.WriteMessage(7, phoneEventFieldNames[3], KeyEvent);
		}
	}

	public override int GetHashCode()
	{
		int num = GetType().GetHashCode();
		if (hasType)
		{
			num ^= type_.GetHashCode();
		}
		if (hasMotionEvent)
		{
			num ^= motionEvent_.GetHashCode();
		}
		if (hasGyroscopeEvent)
		{
			num ^= gyroscopeEvent_.GetHashCode();
		}
		if (hasAccelerometerEvent)
		{
			num ^= accelerometerEvent_.GetHashCode();
		}
		if (hasDepthMapEvent)
		{
			num ^= depthMapEvent_.GetHashCode();
		}
		if (hasOrientationEvent)
		{
			num ^= orientationEvent_.GetHashCode();
		}
		if (hasKeyEvent)
		{
			num ^= keyEvent_.GetHashCode();
		}
		return num;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is PhoneEvent phoneEvent))
		{
			return false;
		}
		if (hasType != phoneEvent.hasType || (hasType && !type_.Equals(phoneEvent.type_)))
		{
			return false;
		}
		if (hasMotionEvent != phoneEvent.hasMotionEvent || (hasMotionEvent && !motionEvent_.Equals(phoneEvent.motionEvent_)))
		{
			return false;
		}
		if (hasGyroscopeEvent != phoneEvent.hasGyroscopeEvent || (hasGyroscopeEvent && !gyroscopeEvent_.Equals(phoneEvent.gyroscopeEvent_)))
		{
			return false;
		}
		if (hasAccelerometerEvent != phoneEvent.hasAccelerometerEvent || (hasAccelerometerEvent && !accelerometerEvent_.Equals(phoneEvent.accelerometerEvent_)))
		{
			return false;
		}
		if (hasDepthMapEvent != phoneEvent.hasDepthMapEvent || (hasDepthMapEvent && !depthMapEvent_.Equals(phoneEvent.depthMapEvent_)))
		{
			return false;
		}
		if (hasOrientationEvent != phoneEvent.hasOrientationEvent || (hasOrientationEvent && !orientationEvent_.Equals(phoneEvent.orientationEvent_)))
		{
			return false;
		}
		if (hasKeyEvent != phoneEvent.hasKeyEvent || (hasKeyEvent && !keyEvent_.Equals(phoneEvent.keyEvent_)))
		{
			return false;
		}
		return true;
	}

	public override void PrintTo(TextWriter writer)
	{
		GeneratedMessageLite<PhoneEvent, Builder>.PrintField("type", hasType, type_, writer);
		GeneratedMessageLite<PhoneEvent, Builder>.PrintField("motion_event", hasMotionEvent, motionEvent_, writer);
		GeneratedMessageLite<PhoneEvent, Builder>.PrintField("gyroscope_event", hasGyroscopeEvent, gyroscopeEvent_, writer);
		GeneratedMessageLite<PhoneEvent, Builder>.PrintField("accelerometer_event", hasAccelerometerEvent, accelerometerEvent_, writer);
		GeneratedMessageLite<PhoneEvent, Builder>.PrintField("depth_map_event", hasDepthMapEvent, depthMapEvent_, writer);
		GeneratedMessageLite<PhoneEvent, Builder>.PrintField("orientation_event", hasOrientationEvent, orientationEvent_, writer);
		GeneratedMessageLite<PhoneEvent, Builder>.PrintField("key_event", hasKeyEvent, keyEvent_, writer);
	}

	public static PhoneEvent ParseFrom(ByteString data)
	{
		return CreateBuilder().MergeFrom(data).BuildParsed();
	}

	public static PhoneEvent ParseFrom(ByteString data, ExtensionRegistry extensionRegistry)
	{
		return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
	}

	public static PhoneEvent ParseFrom(byte[] data)
	{
		return CreateBuilder().MergeFrom(data).BuildParsed();
	}

	public static PhoneEvent ParseFrom(byte[] data, ExtensionRegistry extensionRegistry)
	{
		return CreateBuilder().MergeFrom(data, extensionRegistry).BuildParsed();
	}

	public static PhoneEvent ParseFrom(Stream input)
	{
		return CreateBuilder().MergeFrom(input).BuildParsed();
	}

	public static PhoneEvent ParseFrom(Stream input, ExtensionRegistry extensionRegistry)
	{
		return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
	}

	public static PhoneEvent ParseDelimitedFrom(Stream input)
	{
		return CreateBuilder().MergeDelimitedFrom(input).BuildParsed();
	}

	public static PhoneEvent ParseDelimitedFrom(Stream input, ExtensionRegistry extensionRegistry)
	{
		return CreateBuilder().MergeDelimitedFrom(input, extensionRegistry).BuildParsed();
	}

	public static PhoneEvent ParseFrom(ICodedInputStream input)
	{
		return CreateBuilder().MergeFrom(input).BuildParsed();
	}

	public static PhoneEvent ParseFrom(ICodedInputStream input, ExtensionRegistry extensionRegistry)
	{
		return CreateBuilder().MergeFrom(input, extensionRegistry).BuildParsed();
	}

	private PhoneEvent MakeReadOnly()
	{
		return this;
	}

	public static Builder CreateBuilder()
	{
		return new Builder();
	}

	public override Builder ToBuilder()
	{
		return CreateBuilder(this);
	}

	public override Builder CreateBuilderForType()
	{
		return new Builder();
	}

	public static Builder CreateBuilder(PhoneEvent prototype)
	{
		return new Builder(prototype);
	}
}
