using System;
using System.Collections.Generic;
using System.Text;

namespace Algorithm
{
    class Pos
    {
        public int X;
        public int Y;
        public Pos(int _posy, int _posx)
        {
            Y = _posy;
            X = _posx;
            
        }
        

    }
    
    public class Player
    {
        //인스턴스 변수
        public enum Dir
        {
            Up = 0,//->3
            Left = 1,//0
            Down = 2,//1
            Right = 3,//2
            
        }

        //현재 바라보고 있는 방향 기준으로 앞으로 이동 
        int[] frontX = { 0, -1, 0, 1 };
        int[] frontY = { -1, 0, 1, 0 };

        int[] rightX = { 1, 0, -1, 0 };
        int[] rightY = { 0, -1, 0, 1 };


        Board _board;

        public int PosY = 0;
        public int PosX = 0;

        public int DesY = 0;
        public int DesX = 0;

        int _dir = (int)Dir.Up;
        
        List<Pos> _points = new List<Pos>();
       
        

        public void Initialize(int _posy, int _posx, int _desy, int _desx, Board board)
        {
            _board = board;

            PosY = _posy;
            PosX = _posx;

            DesY = _desy;
            DesX = _desx;


            BFS();

        }

        int[] deltaY = new int[] { -1, 0, 1, 0 };
        int[] deltaX = new int[] { 0, -1, 0, 1 };

        public void BFS()
        {
            bool[,] found = new bool[_board.Size , _board.Size];
            
            int nowY = PosY;
            int nowX = PosX;

            int nextY;
            int nextX;

            Queue<Pos> queue = new Queue<Pos>();
            queue.Enqueue(new Pos(nowY, nowX));
            Pos[,] parent = new Pos[_board.Size,_board.Size];
            parent[nowY, nowX] = new Pos(nowY, nowX);//parent[1,1] = new Pos(1,1)
            found[nowY, nowX] = true;

            Pos pos;

            while (queue.Count > 0)
            {
                pos = queue.Dequeue();

                nowY = pos.Y;
                nowX = pos.X;

                for (int i = 0; i < 4; i++)
                {
                    nextY = nowY + deltaY[i];
                    nextX = nowX + deltaX[i];
                    //1) 갈 수 있는 정점 중에서
                    //2) 가보지 않은 곳을 큐에 저장함
                    if (nextY < 0 || nextY > _board.Size - 1 || nextX < 0 || nextX > _board.Size - 1)
                        continue;
                    if (_board.Tiles[nextY, nextX] == TileType.Wall)
                        continue;
                    if (found[nextY, nextX])
                        continue;
                    

                    queue.Enqueue(new Pos(nextY, nextX));
                    found[nextY, nextX] = true;
                    parent[nextY, nextX] = new Pos(nowY, nowX);


                }
            }
               
            int y = DesY;
            int x = DesX;

            while (true)
                {
                    
                    _points.Add(new Pos(y, x));
                    Pos _pos = parent[y, x];

                    y = _pos.Y;
                    x = _pos.X;

                if (parent[y, x].Y == y && parent[y, x].X == x)
                    break;

                }

                _points.Add(new Pos(y, x));
                _points.Reverse();

     
        }
        
       /* public void RightHand()
        {
            while (PosY != DesY || PosX != DesX)
            {

                //1 . 현재 바라보는 방향 기준으로 오른쪽으로 전진 할 수 있는지 확인
                if (_board.Tiles[PosY + rightY[_dir], PosX + rightX[_dir]] == TileType.Empty)
                {
                    // 바라보는 방향을 오른쪽으로 90도 회전
                    _dir = (_dir - 1 + 4) % 4;
                    // 앞으로 1보 전진
                    PosY = PosY + frontY[_dir];
                    PosX = PosX + frontX[_dir];
                    //새로운 좌표 추가
                    _points.Add(new Pos(PosY, PosX));

                }//2 . 현재 바라보는 방향 기준으로 앞으로 전진할 수 있는지 확인
                else if (_board.Tiles[PosY + frontY[_dir], PosX + frontX[_dir]] == TileType.Empty)
                {
                    //앞으로 전진
                    PosY = PosY + frontY[_dir];
                    PosX = PosX + frontX[_dir];

                    //새로운 좌표 추가
                    _points.Add(new Pos(PosY, PosX));


                }//3 . 왼쪽으로 90도 회전
                else
                {
                    _dir = (_dir + 1 + 4) % 4;

                }
            }
        }*/

        Random rand = new Random();
        int sumTick = 0;
        int moveTick = 10;
        int lastIndex = 0;
        public void Update(int deltaTick )
        {
            
            sumTick += deltaTick;
            if (lastIndex > _points.Count - 1 )
                return;
            if (sumTick >= moveTick)//0.1초
            {
                
                sumTick = 0;
                PosX = _points[lastIndex].X;
                PosY = _points[lastIndex].Y;
                lastIndex++;
                                
                
            }
        }
    }
}
