using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
    public class GameState
    {
        // Các thuộc tính để lưu trữ thông tin về trạng thái của trò chơi
        public int Rows { get; }
        public int Cols { get; }
        public GridValue[,] Grid { get; }
        public Direction Dir { get; private set; }
        public int Score { get; private set; }
        public bool GameOver { get; private set; }

        // Danh sách liên kết để lưu trữ các thay đổi hướng và vị trí của rắn
        private readonly LinkedList<Direction> dirChanges = new LinkedList<Direction>();
        private readonly LinkedList<Postition> snakePostitions = new LinkedList<Postition>();

        private readonly Random random = new Random();

        /// <summary>
        /// Khởi tạo trạng thái của trò chơi với số hàng và cột cho trước.
        /// </summary>
        /// <param name="rows">Số hàng của bảng.</param>
        /// <param name="cols">Số cột của bảng.</param>
        /// 👈(ﾟヮﾟ👈)
        
        public GameState(int rows, int cols) 
        {
            Rows = rows;
            Cols = cols;
            Grid = new GridValue[rows, cols];
            Dir = Direction.Right;

            AddSnake();
            AddFood();
        }
        private void AddSnake() 
        {
            int r = Rows / 2;
            for (int c = 1; c <= 3; c++)
            {
                Grid[r, c] = GridValue.Snake;
                snakePostitions.AddFirst(new Postition(r, c));
            }
        }
        private IEnumerable<Postition> EmptyPositions()
        {
            for (int r = 0; r < Rows; r++) 
            {
                for (int c = 0; c < Cols; c++)
                {
                    if (Grid[r, c] == GridValue.Empty)
                    {
                        yield return new Postition(r, c);
                    }
                }
            }
        }

        private void AddFood()
        {
            List<Postition> empty = new List<Postition>(EmptyPositions());
            if (empty.Count == 0)
            {
                return;
            }

            Postition pos = empty[random.Next(empty.Count)];
            Grid[pos.Row, pos.Col] = GridValue.Food;
        }


        public Postition HeadPosition() 
        {
            return snakePostitions.First.Value;
        }

        public Postition TailPosition() 
        {
            return snakePostitions.Last.Value;
        }

        public IEnumerable<Postition> SnakePositions() 
        {
            return snakePostitions;
        }

        private void AddHead(Postition pos) 
        {
            snakePostitions.AddFirst(pos);
            Grid[pos.Row, pos.Col] = GridValue.Snake;
        }

        private void RemoveTail() 
        {
            Postition tail = snakePostitions.Last.Value;
            Grid[tail.Row, tail.Col] = GridValue.Empty;
            snakePostitions.RemoveLast();
        }

        private Direction GetLastDirection()
        {
            if (dirChanges.Count == 0) 
            {
                return Dir;
            }
            return dirChanges.Last.Value;
        }

        private bool CanChangeDirection(Direction newDir) 
        {
                if(dirChanges.Count == 2) 
                {
                    return false;
                }

                Direction lastDir = GetLastDirection();
            return newDir != lastDir && newDir != lastDir.Opposite();

        }
        public void ChangeDirection(Direction dir) 
        {
            // if can change direction
            if (CanChangeDirection(dir)) 
            {
                dirChanges.AddLast(dir);
            }
          

        }

        private bool OutsideGrid(Postition pos) 
        {
          return pos.Row < 0 || pos.Row >= Rows || pos.Col < 0 || pos.Col >= Cols;
        }

        private GridValue WillHit(Postition newHeadPos) 
        {
            if (OutsideGrid(newHeadPos)) 
            {
                return GridValue.Outside;
            }

            if (newHeadPos == TailPosition()) 
            {
                return GridValue.Empty;
            }

            return Grid[newHeadPos.Row, newHeadPos.Col];
        }

        public void Move() 
        {
            if (dirChanges.Count > 0) 
            {
                Dir = dirChanges.First.Value;
                dirChanges.RemoveFirst();
            }

            Postition newHeadPos = HeadPosition().Translate(Dir);
            GridValue hit = WillHit(newHeadPos);

            if (hit == GridValue.Outside || hit == GridValue.Snake)
            {
                GameOver = true;
            }
            else if (hit == GridValue.Empty)
            {
                RemoveTail();
                AddHead(newHeadPos);
            }
            else if (hit == GridValue.Food)
            {
               AddHead(newHeadPos);
                Score++;
                AddFood();
            }

        }
    }
}
