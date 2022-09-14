using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

    class Player
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

        //int _dir = (int)Dir.Up;

        List<Pos> _points = new List<Pos>();



        public void Initialize(int _posy, int _posx, int _desy, int _desx, Board board)
        {
            _board = board;

            PosY = _posy;
            PosX = _posx;

            DesY = _desy;
            DesX = _desx;


            AStar();

        }
        //우선순위 큐 
        struct PQNode : IComparable<PQNode>
        {
            public int F;
            public int G;
            public int Y;
            public int X;

            public int CompareTo(PQNode other)
            {
                if (F == other.F)
                    return 0;
                return F < other.F ? -1 : 1;
            }
        }
        //AStar 알고리즘
        public void AStar()
        {
            //점수 매기기
            //F = G + H
            //F = 최종 점수(작을수록 좋음)
            //G = 시작점에서 해당 좌표까지 이동하는데 드는 비용(작을수록 좋음)
            //H = 목적지에서 떨어진 거리(작을수록 좋음)

            //(y,x) 이미 방문 했는지 여부
            bool[,] closed = new bool[_board.Size, _board.Size];

            //(y,x) 가 예약 되었는지 -> 비용 
            int[,] open = new int[_board.Size, _board.Size];

            for (int y = 0; y <_board.Size; y++)
            {
                for (int x = 0; x < _board.Size; x++)
                    open[y, x] = Int32.MaxValue;
            }

            Pos[,] parent = new Pos[_board.Size, _board.Size];
            //시작점 -> 자기자신이 부모
            parent[PosY, PosX] = new Pos(PosX, PosY);

            PriorityQueue<PQNode> pq = new PriorityQueue<PQNode>();

            pq.push(new PQNode() { F = Math.Abs(DesY - PosY) + Math.Abs( DesX - PosX ) , G = 0, Y = PosY, X = PosX });
            //제일 좋은 후보를 찾는다
            while(pq.count() > 0)
            {
                
                PQNode node = pq.pop();

                if (closed[node.Y, node.X])
                    continue;
                if (node.Y == DesY && node.X == DesX)
                    break;

                //방문한다
                closed[node.Y, node.X] = true;
                int[] cost = new int[] { 1, 1, 1, 1 };
                //상하 좌우 등 이동 할 수 있는 좌표인지 확인해서 예약(open)한다
                for (int i = 0; i<deltaY.Length; i++)
                {
                    int nextY = node.Y + deltaY[i];
                    int nextX = node.X + deltaX[i];

            

                    //유효범위를 벗어났으면 스킵
                    if (nextY < 0 || nextY > _board.Size - 1 || nextX < 0 || nextX > _board.Size - 1)
                        continue;
                    //벽으로 막혀서 갈 수 없으면 스킵
                    if (_board.Tiles[nextY, nextX] == Board.TileType.Wall)
                        continue;
                    //이미 방문한 곳으면 스킵
                    if (closed[nextY, nextX])
                        continue;
                    //비용 계산
                    int g = node.G + cost[i];
                    int h = Math.Abs(DesY - nextY) + Math.Abs(DesX - nextX);

                    //이미 더 적은 비용으로 예약되었다면 스킵
                    if (open[nextY, nextX] < g + h)
                        continue;
                    //예약 진행
                    open[nextY, nextX] = g + h;
                    pq.push(new PQNode() { F = g+h, G = g, Y = nextY, X = nextX });

                    //부모 기억하기
                    parent[nextY, nextX] = new Pos(node.Y, node.X);

                }
                

                 

            }
            //_points에 방문 노드 저장

            PathFromParent(parent);


        }
        public void PathFromParent(Pos[,] parent)
        {
            
            int y = DesY;
            int x = DesX;

            while(parent[y,x].Y != y || parent[y,x].X != x)
            {
                _points.Add(new Pos(y, x));
                Pos _pos = parent[y, x];
                y = _pos.Y;
                x = _pos.X;

            }
            _points.Add(new Pos(y, x));
            _points.Reverse();
        }

        int[] deltaY = new int[] { -1, 0, 1, 0 };
        int[] deltaX = new int[] { 0, -1, 0, 1 };

        public void BFS()
        {
            bool[,] found = new bool[_board.Size, _board.Size];

            int nowY = PosY;
            int nowX = PosX;

            int nextY;
            int nextX;

            Queue<Pos> queue = new Queue<Pos>();
            queue.Enqueue(new Pos(nowY, nowX));
            Pos[,] parent = new Pos[_board.Size, _board.Size];
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
                    if (_board.Tiles[nextY, nextX] == Board.TileType.Wall)
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
        public void Update(int deltaTick)
        {

            sumTick += deltaTick;
            //목적지에 도착해서 다시 길을 찾도록

            if (lastIndex > _points.Count - 1)
            {
                lastIndex = 0;
                _points.Clear();

                _board.Initialize(_board.Size, this );
                Initialize(1, 1, _board.Size -2, _board.Size -2  , _board);
                
               

            }
               
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