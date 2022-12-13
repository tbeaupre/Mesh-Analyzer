using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NeighborManager
{
    public static void SetValidNeighbors(ModuleSet moduleSet)
    {
        foreach (Module module1 in moduleSet.modules)
        {
            module1.validNeighbors = new ValidNeighbors();

            bool shouldAddTopNeighbors = module1.name == FaceData.INTERNAL_NAME ||
                module1.sockets.top != "0s";
            if (!shouldAddTopNeighbors)
            {
                if (module1.isFacingUp)
                    module1.validNeighbors.top.Add(FaceData.EXTERNAL_NAME);
                else
                    module1.validNeighbors.top.Add(FaceData.INTERNAL_NAME);

            }

            bool shouldAddBottomNeighbors = module1.name == FaceData.EXTERNAL_NAME ||
                module1.sockets.bottom != "0s";
            if (!shouldAddBottomNeighbors)
            {
                if (module1.isFacingUp)
                    module1.validNeighbors.bottom.Add(FaceData.INTERNAL_NAME);
                else
                    module1.validNeighbors.bottom.Add(FaceData.EXTERNAL_NAME);
            }

            foreach (Module module2 in moduleSet.modules)
            {
                if (AreModuleFacesCompatible(module1, module2, Direction.Back))
                    module1.validNeighbors.back.Add(module2.name);

                if (AreModuleFacesCompatible(module1, module2, Direction.Left))
                    module1.validNeighbors.left.Add(module2.name);

                if (AreModuleFacesCompatible(module1, module2, Direction.Right))
                    module1.validNeighbors.right.Add(module2.name);

                if (shouldAddTopNeighbors && module2.name != FaceData.EXTERNAL_NAME && AreSocketsCompatible(module1.sockets.top, module2.sockets.bottom))
                    module1.validNeighbors.top.Add(module2.name);

                if (shouldAddBottomNeighbors && module2.name != FaceData.INTERNAL_NAME && AreSocketsCompatible(module1.sockets.bottom, module2.sockets.top))
                    module1.validNeighbors.bottom.Add(module2.name);
            }
        }
    }

    private static bool AreModuleFacesCompatible(Module module1, Module module2, Direction dir)
    {
        if (!AreSocketsCompatible(module1.sockets.GetSocketInDirection(dir), module2.sockets.GetSocketInDirection(dir)))
            return false;

        if (module1.meshName == module2.meshName && module1.name != module2.name && module1.rotation == module2.rotation) // Internal/External Combo
            return false;
        if (module1.name == FaceData.EXTERNAL_NAME && module2.meshName.Length != 0)
            return ExternalCheck(module2, dir);
        if (module2.name == FaceData.EXTERNAL_NAME && module1.meshName.Length != 0)
            return ExternalCheck(module1, dir);
        if (module1.name == FaceData.INTERNAL_NAME && module2.meshName.Length != 0)
            return !ExternalCheck(module2, dir);
        if (module2.name == FaceData.INTERNAL_NAME && module1.meshName.Length != 0)
            return !ExternalCheck(module1, dir);

        if (IsStraightCliff(module1) && IsStraightCliff(module2) && !IsCliffBackInDir(module1, dir))
            return false;

        return true;
    }

    private static bool IsCliffBackInDir(Module module, Direction dir)
    {
        string cliffStr = module.meshName.Split('-')[1]; // Ex: FFF-CCE-R
        switch (dir)
        {
            case Direction.Back:
                return cliffStr[(-module.rotation + 3) % 3] != 'E' && cliffStr[(1 - module.rotation + 3) % 3] != 'E';
            case Direction.Right:
                return cliffStr[(1 - module.rotation + 3) % 3] != 'E' && cliffStr[(2 - module.rotation + 3) % 3] != 'E';
            default: // Direction.Left
                return cliffStr[(2 - module.rotation + 3) % 3] != 'E' && cliffStr[(-module.rotation + 3) % 3] != 'E';
        }
    }

    private static bool IsStraightCliff(Module module)
    {
        if (module.meshName.Length == 0)
            return false;

        string cliffStr = module.meshName.Split('-')[1]; // Ex: FFF-CCE-R
        return cliffStr.GroupBy(c => c).First(c => c.Key == 'E').Count() == 1;
    }

    // Checks if a blank face should be considered External.
    private static bool ExternalCheck(Module module, Direction dir)
    {
        if (module.traversalSet.top)
            return false;

        string cliffStr = module.meshName.Split('-')[1]; // Ex: FFF-CCE-R
        switch (dir)
        {
            case Direction.Back:
                return cliffStr[(-module.rotation + 3) % 3] == 'E' || cliffStr[(1 - module.rotation + 3) % 3] == 'E';
            case Direction.Right:
                return cliffStr[(1 - module.rotation + 3) % 3] == 'E' || cliffStr[(2 - module.rotation + 3) % 3] == 'E';
            default: // Direction.Left
                return cliffStr[(2 - module.rotation + 3) % 3] == 'E' || cliffStr[(-module.rotation + 3) % 3] == 'E';
        }
    }

    private static bool AreSocketsCompatible(string socket1, string socket2)
    {
        bool isSymmetrical = socket1.EndsWith("s");
        bool isVertical = socket1.StartsWith("v");

        if (isSymmetrical || isVertical)
            return socket1 == socket2;

        return socket1 == socket2 + "f" || socket2 == socket1 + "f";
    }
}
