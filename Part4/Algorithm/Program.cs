using System;
using Algorithm;


namespace Algorithm
{
    class Program
    {
        static void Main(string[] args)
        {

            //입력
            Player player = new Player();
            Board board = new Board();
            

            board.Initialize(25,  player);
            player.Initialize(1, 1, board.Size - 2, board.Size - 2, board);

            Console.CursorVisible = false;
            
            const int WAIT_TICK = 1000 / 30;
            int lastTick = 0;
            int deltaTick;

            while (true)

            {
                #region 프레임 관리 
                //만약 경과한 시간이 1/30 초 (1000/30 milliseconds)보다 작다면
                int currentTick = System.Environment.TickCount;
                if (currentTick - lastTick < WAIT_TICK)
                    continue;
                deltaTick = currentTick - lastTick;
                player.Update(deltaTick);
                lastTick = currentTick;
                #endregion

                
                //로직
                //렌더링
                
                Console.SetCursorPosition(0, 0);

                board.Render();

            }
        }

    }
}
