using System;
using System.Collections.Generic;
using System.Text;

namespace Algorithm
{
    class PriorityQueue<T> where T : IComparable<T>//단 같은 클래스 객체와 비교가능한 
    {
        public List<T> _heap = new List<T>();

        public void push(T data)
        {
            _heap.Add(data);

            int now = _heap.Count - 1;
            //위쪽을 향해 도장 깨기
            while (now > 0)
            {
                //next 가 크면 종료
                int next = (now - 1) / 2;
                if (_heap[now].CompareTo(_heap[next]) > 0)
                    break;


                T temp = _heap[next];
                _heap[next] = _heap[now];
                _heap[now] = temp;


                now = next;

            }

        }
        public T pop()
        {

            //가장 우선순위가 높은 것 
            T ret = _heap[0];


            int lastIndex = _heap.Count - 1;
            _heap[0] = _heap[lastIndex];
            _heap.RemoveAt(lastIndex);
            //
            int now = 0;
            while (true)
            {
                //밑으로 도장깨기
                //next-> 가려는 인덱스
                int next = now;
                int left = (2 * now) + 1;
                int right = (2 * now) + 2;

                //왼쪽 값이 현재 값보다 크면 
                //now가 인덱스가 큼
                if (left < _heap.Count && _heap[next].CompareTo(_heap[left]) > 0)
                {
                    next = left;
                }
                //오른값이 현재 값보다 크면
                //now가 인덱스가 큼
                if (right < _heap.Count && _heap[next].CompareTo(_heap[right]) > 0)
                {
                    next = right;
                }

                if (now == next)
                    break;

                T temp = _heap[next];
                _heap[next] = _heap[now];
                _heap[now] = temp;

                now = next;


            }
            return ret;


        }
        public int count()
        {
            return _heap.Count;
        }



    }
}
