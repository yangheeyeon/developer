using System;
using System.Collections.Generic;

namespace Exercise
{
    class Graph
    {
        //가중치 그래프
        int[,] adj = new int[6, 6]
        {   {-1,15,-1,35,-1,-1},
            {15,-1, 5,10,-1,-1},
            {-1, 5,-1,-1,-1,-1},
            {35,10,-1,-1, 1,-1},
            {-1,-1,-1, 5,-1, 5},
            {-1,-1,-1,-1, 5,-1}
        };

        //다익스트라 알고리즘
        public void Dijkstra(int start)
        {
            visited = new bool[6];

            //now로 부터 가장 가까운 정점을 방문하기

            int[] parent = new int[6];
            int[] distances = new int[6];
            Array.Fill(distances, Int32.MaxValue);
            parent[start] = 0;
            distances[start] = 0;




            while (true)
            {
                int closest = Int32.MaxValue;
                int now = -1;

                //1) 예약된 정점 중에서 최단인 정점 방문
                //2) 연결된 정점들 예약해 놓기

                //1) 예약된 정점 중에서 최단인 정점 방문
                for (int i = 0; i < 6; i++)
                {
                    //방문한 정점이면 스킵(방문한 정점제외하고 최단 거리 구하기)
                    if (visited[i])
                        continue;
                    //예약되지 않은 정점이거나 최단 거리보다 멀리 있는 정점은 스킵
                    if (distances[i] == Int32.MaxValue || distances[i] >= closest)
                        continue;

                    closest = distances[i];
                    now = i;

                    visited[now] = true;
                    Console.WriteLine(now);
                    //새롭게 갈 만한 정점을 발견 못하면(이미 모두 방문 했다면) ->반복문 종료
                    if (now == -1)
                        break;
                    //2) 방문한 정점에서 연결된 정점들 예약해 놓기
                    for (int next = 0; next < 6; next++)
                    {
                        //연결되어 있지 않은 정점은 스킵
                        if (adj[now, next] == -1)
                            continue;
                        //이미 방문한 정점은 스킵
                        if (visited[next])
                            continue;

                        int newDist = distances[now] + adj[now, next];

                        if (newDist < distances[next])
                        {
                            distances[next] = newDist;

                        }
                        parent[next] = now;





                    }
                }
            }

        }

        //1)우선 now부터 방문하고
        //2)now와 연결된 정점들을 확인해서, 아직 미발견(미방문)상태라면 방문함

        //방문한 정점 저장
        bool[] visited = new bool[6];
        public void DFS(int now)
        {
            Console.WriteLine(now);

            for (int i = 0; i < 6; i++)
            {
                visited[now] = true;

                if (adj[now, i] == 1)
                {
                    if (visited[i])
                        continue;
                    DFS(i);
                }


            }


        }

        public void SearchAll()
        {
            visited = new bool[6];
            for (int now = 0; now < 6; now++)
            {
                if (visited[now])
                    continue;


                DFS(now);
            }

        }


        //now와 연결된 정점들을 찾아내서
        //아직 미발견 상태라면 큐에 추가함
        public void BFS(int now)
        {
            bool[] found = new bool[6];
            Queue<int> q = new Queue<int>();//큐queue

            q.Enqueue(now);
            found[now] = true;

            while (q.Count > 0)
            {
                now = q.Dequeue();
                Console.WriteLine(now);
                for (int next = 0; next < 6; next++)
                {
                    //now와 연결된 정점들 중에서
                    if (adj[now, next] == 0)
                        continue;

                    //미발견(미방문)상태의 정점을 방문한다
                    if (found[next])
                        continue;
                    found[next] = true;
                    q.Enqueue(next);


                }


            }



        }
    }
    /*class Program
    {

        static void Main(string[] args)
        {
            Graph graph = new Graph();
            graph.Dijkstra(0);

        }
    }*/
}
