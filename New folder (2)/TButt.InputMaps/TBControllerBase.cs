using System.Collections.Generic;
using UnityEngine;

namespace TButt.InputMaps;

[ExecuteInEditMode]
public abstract class TBControllerBase<T>
{
	protected string name;

	protected string fileName;

	protected TBInput.Controller type;

	protected bool supportsRumble;

	protected bool loaded;

	protected List<TBInput.ButtonDef<T>> loadedButtonDefs;

	protected TBInput.ButtonLookupTable<T> lookupTable;

	protected HandTrackingOffsets trackingOffsets;

	public readonly float PointerTiltAngle;

	public virtual List<TBInput.ButtonDef<T>> GetDefaultDefs()
	{
		return new List<TBInput.ButtonDef<T>>();
	}

	protected virtual void Initialize()
	{
		trackingOffsets = GetDefaultTrackingOffsets();
		loadedButtonDefs = TBInput.LoadButtonDefs(GetDefaultDefs(), fileName);
		lookupTable = TBInput.NewLookupTableFromDefs(loadedButtonDefs);
		loaded = true;
	}

	public virtual string GetName()
	{
		return name;
	}

	public virtual string GetFileName()
	{
		return fileName;
	}

	public virtual TBInput.Controller GetControllerType()
	{
		return type;
	}

	public virtual bool HasRumbleSupport()
	{
		return supportsRumble;
	}

	public virtual bool IsLoaded()
	{
		return loaded;
	}

	public virtual List<TBInput.ButtonDef<T>> GetLoadedButtonDefs()
	{
		return loadedButtonDefs;
	}

	public virtual TBInput.ButtonLookupTable<T> GetLookupTable()
	{
		return lookupTable;
	}

	public virtual HandTrackingOffsets GetTrackingOffsets()
	{
		return trackingOffsets;
	}

	protected virtual HandTrackingOffsets GetDefaultTrackingOffsets()
	{
		return new HandTrackingOffsets
		{
			positionOffset = Vector3.zero,
			rotationOffset = Vector3.zero
		};
	}
}
