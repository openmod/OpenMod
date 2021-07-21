using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using OpenMod.UnityEngine.Transforms;
using SDG.Unturned;
using UnityEngine;

namespace OpenMod.Unturned.Building
{
    public class UnturnedStructureBuildable : UnturnedBuildable
    {
        public StructureData StructureData
        {
            get
            {
                return StructureDrop.GetServersideData();
            }
        }

        public StructureDrop StructureDrop { get; }

        [Obsolete]
        public UnturnedStructureBuildable(StructureData data, StructureDrop drop) : this(drop)
        {
            if (data != drop.GetServersideData())
            {
                throw new Exception("StructureData is not equal to the drop data");
            }
        }

        public UnturnedStructureBuildable(StructureDrop drop) : base(
            new UnturnedBuildableAsset(drop.GetServersideData().structure.asset),
            new UnityTransform(drop.model),
            new UnturnedBuildableState(drop.GetServersideData().structure),
            new UnturnedBuildableOwnership(drop.GetServersideData()),
            drop.instanceID.ToString())
        {
            StructureDrop = drop;
        }

        public override Task DestroyAsync()
        {
            async UniTask DestroyTask()
            {
                await UniTask.SwitchToMainThread();

                if (Regions.tryGetCoordinate(StructureDrop.GetServersideData().point, out var x, out var y))
                {
                    return;
                }

                StructureManager.destroyStructure(StructureDrop, x, y, Vector3.zero);
            }

            return DestroyTask().AsTask();
        }
    }
}