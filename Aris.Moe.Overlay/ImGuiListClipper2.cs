using ImGuiNET;

namespace Aris.Moe.Overlay
{
    internal class ImGuiListClipper2
    {
        public float StartPosY;
        public float ItemsHeight;
        public int ItemsCount, StepNo, DisplayStart, DisplayEnd;

        public ImGuiListClipper2(int items_count = -1, float items_height = -1.0f)
        {
            Begin(items_count, items_height);
        }

        public unsafe void Begin(int count, float items_height = -1.0f)
        {
            StartPosY = ImGuiNative.igGetCursorPosY();
            ItemsHeight = items_height;
            ItemsCount = count;
            StepNo = 0;
            DisplayEnd = DisplayStart = -1;
            if (ItemsHeight > 0.0f)
            {
                int dispStart, dispEnd;
                ImGuiNative.igCalcListClipping(ItemsCount, ItemsHeight, &dispStart, &dispEnd);
                DisplayStart = dispStart;
                DisplayEnd = dispEnd;
                if (DisplayStart > 0)
                    //SetCursorPosYAndSetupDummyPrevLine(StartPosY + DisplayStart * ItemsHeight, ItemsHeight); // advance cursor
                    ImGuiNative.igSetCursorPosY(StartPosY + DisplayStart * ItemsHeight);
                StepNo = 2;
            }
        }
    }
}