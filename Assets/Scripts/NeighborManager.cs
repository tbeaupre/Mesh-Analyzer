using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                if (AreSocketsCompatible(module1.sockets.back, module2.sockets.back))
                    module1.validNeighbors.back.Add(module2.name);

                if (AreSocketsCompatible(module1.sockets.left, module2.sockets.left))
                    module1.validNeighbors.left.Add(module2.name);

                if (AreSocketsCompatible(module1.sockets.right, module2.sockets.right))
                    module1.validNeighbors.right.Add(module2.name);

                if (shouldAddTopNeighbors && module2.name != FaceData.EXTERNAL_NAME && AreSocketsCompatible(module1.sockets.top, module2.sockets.bottom))
                    module1.validNeighbors.top.Add(module2.name);

                if (shouldAddBottomNeighbors && module2.name != FaceData.INTERNAL_NAME && AreSocketsCompatible(module1.sockets.bottom, module2.sockets.top))
                    module1.validNeighbors.bottom.Add(module2.name);
            }
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
