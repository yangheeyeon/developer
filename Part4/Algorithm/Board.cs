using System;
using System.Collections.Generic;
using System.Text;

namespace Algorithm
{

    public enum TileType
    {
        Empty,
        Wall,
    }
    public class Board
    {
        Player _player;
        const char CIRCLE = '\u25cf';
        public int Size { get; private set; }
        public TileType[,] Tiles { get; private set; }

        public void Initialize(int _size , Player player)
        {

            _player = player;

            //보드 초기화
            if (Size % 2 != 0)//보드의 사이즈N 는 홀수
                return;
            Size = _size;
            Tiles = new TileType[Size, Size];

            GenerateBySideWinder();


        }
        void GenerateBySideWinder()
        {
            Random rand = new Random();
            //길을 다 막아 버리는 작업
            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    if (y % 2 == 0 || x % 2 == 0)
                    {
                        Tiles[y, x] = TileType.Wall;
                    }
                    else 
                    { 
                        Tiles[y, x] = TileType.Empty; 
                    }

                }
            }

            //벽을 뚫는 작업
            for (int y = 0; y < Size; y++)
            {
                int count = 1;
                for (int x = 0; x < Size; x++)
                {
                    if (y % 2 == 0 || x % 2 == 0)
                        continue;

                    if (y == Size - 2 && x == Size - 2)
                        continue;

                    if (y == Size - 2)
                    {
                        Tiles[y, x+1] = TileType.Empty;
                        continue;
                    }
                    if (x == Size - 2)
                    {
                        Tiles[y + 1, x] = TileType.Empty;
                        continue;
                    }



                    if (rand.Next(0, 2) == 0)
                    {
                        //오른쪽으로 벽 뚫기
                        Tiles[y, x + 1] = TileType.Empty;
                        count++;
                    }
                    else
                    {
                        //아래로 벽 뚫기
                        int randIndex = rand.Next(0, count );
                        Tiles[y + 1, x-randIndex*2] = TileType.Empty;
                        count = 1;
                    }
                }
            }

        }
           /* void GenerateByBinaryTree()
            {


                //길을 다 막아버리는 작업

                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        if (y % 2 == 0 || x % 2 == 0)
                        {
                            Tiles[y, x] = TileType.Wall;
                        }

                        else { Tiles[y, x] = TileType.Empty; }

                    }
                }
                //랜덤으로 길을 뚫는 작업
                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        if (y % 2 == 0 || x % 2 == 0)
                            continue;
                        if (y == size - 2)
                            continue;
                        if (x == size - 2)
                            continue;
                        Random rand = new Random();
                        if (rand.Next(0, 2) == 0)
                        {
                            //오른쪽 벽 뚫기
                            Tiles[y, x + 1] = TileType.Empty;
                        }
                        else
                        {
                            //아래쪽 벽 뚫기
                            Tiles[y + 1, x] = TileType.Empty;
                        }

                    }


                }

            }*/
            public void Render()
            {
                for (int y = 0; y < Size; y++)
                {
                    for (int x = 0; x < Size; x++)
                    {
                    if (_player.PosY == y && _player.PosX == x) {

                        Console.ForegroundColor = ConsoleColor.White;

                    }
                    else if(_player.DesY == y && _player.DesX == x){

                        Console.ForegroundColor = ConsoleColor.Yellow;
                    }
                    else {

                        Console.ForegroundColor = GetTileColor(Tiles[y, x]);

                    }
                           
                        Console.Write(CIRCLE);
                    }
                    Console.WriteLine();
                }

            Console.ForegroundColor = ConsoleColor.White;

            }
            ConsoleColor GetTileColor(TileType type)
            {

                switch (type)
                {
                    case TileType.Wall:
                        return ConsoleColor.Red;
                    case TileType.Empty:
                        return ConsoleColor.Green;
                    default:
                        return ConsoleColor.Green;
                }

            }
        
    }
}
