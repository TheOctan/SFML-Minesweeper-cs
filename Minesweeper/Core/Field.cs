using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Minesweeper.Core
{
    public class Field : Transformable, Drawable
    {
        private Sprite sprite;                  //спрайт
        private Vector2i sizeField;             //размер игрового поля
        private Vector2i tile;                  //номер плитки

        private int width;                      //ширина плитки
        private int maxCountMines;             //максимальное количество мин
        private int countMines;                //количество мин

	    public int Flags => countMines - countFlags;	// возвращает количество не установленных флагов
		private int countFlags;			//количество флагов

        private int[,] grid;           //скрытая сетка
        private int[,] showGrid;       //сетка для отрисовки

        public bool IsGameOver { get; private set; } //статус проигрыша игры
        public bool IsGameWin { get; private set; } //статус выигрыша игры

        public Field(Texture texture) : this(texture, new Vector2i(10, 10), 10)
        {

        }

        public Field(Texture texture, Vector2i sizeField, int countMines)
        {
            sprite = new Sprite(texture);

            width = 32;
            maxCountMines = (sizeField.X * sizeField.Y) / 2;

            this.countMines = countMines > maxCountMines ? maxCountMines : countMines;
            this.sizeField = sizeField;

            grid = new int[sizeField.X + 2, sizeField.Y + 2];
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    grid[i, j] = -2;    // заполняем матрицу значениями -2
                }
            }

            showGrid = new int[sizeField.X + 2, sizeField.Y + 2];
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    showGrid[i, j] = -2;    // заполняем матрицу значениями -2
                }
            }

            IsGameOver = false;
            IsGameWin = false;

            Init();
        }

        public void Update(Vector2i pos, Mouse.Button button)
        {
            if (!IsGameOver && !IsGameWin)          // если не проигрыш и не выйгрыш
            {
				// определение номера ячейки по координатам мыши
				// находим разность между координатой мыши и текущей координатой поля, которое наследуется от Transformable
				// делим на ширину тайла
				// и прибавляем единицу, так как края поля являются пустыми
                int x = (int)((pos.X - Position.X) / width + 1);
                int y = (int)((pos.Y - Position.Y) / width + 1);

	            if (x >= 1 && x <= sizeField.X && y >= 1 && y <= sizeField.Y)		// проверка границ массива
	            {
		            tile.X = x;
		            tile.Y = y;

		            if (button == Mouse.Button.Left)							// если нажата левая кнопка мыши
		            {
			            if (showGrid[tile.X, tile.Y] != 11)						// если текущая ячейка не флаг
			            {
                            showGrid[tile.X, tile.Y] = grid[tile.X, tile.Y];        // отображаем текущий тайл из скрытой сетки
                            Fill(tile.X, tile.Y);                                   // проверяем на наличие пустых ячеек, и расскрываем их при наличии
                        }
		            }
					else if (button == Mouse.Button.Right)                      // инча если нажата правая кнопка мыши
                    {
	                    if (showGrid[tile.X, tile.Y] == 10 && countFlags < countMines)	// если текущая ячейка явдяется заглушкой и количество флажков меньше еоличества мин
	                    {
		                    showGrid[tile.X, tile.Y] = 11;					// текущая ячейка ставится флаг
		                    countFlags++;									// увеличиваем количество флагов
	                    }
	                    else if(showGrid[tile.X, tile.Y] == 11)			// иначе если текущая ячейка равна флажку
	                    {
                            showGrid[tile.X, tile.Y] = 10;				// ставим на ячейку заглушку
                            countFlags--;								// уменьшаем количество флагов
                        }
		            }
	            }
            }


        }   // обновление

        public void Draw(RenderTarget target, RenderStates states)
        {
	        if (IsGameWin)					// если победа
	        {
		        countFlags = countMines;	// приравниваем количество флагов количеству мин
		        for (int i = 1; i <= sizeField.X; i++)
		        {
			        for (int j = 1; j <= sizeField.Y; j++)
			        {
				        if (grid[i, j] == 9)				// если скрытая ячейка является миной
					        showGrid[i, j] = 11;			// то на отображаемую ячейку ставим флаг
			        }
		        }
	        }

	        for (int i = 1; i <= sizeField.X; i++)
	        {
		        for (int j = 1; j <= sizeField.Y; j++)
		        {
			        if (grid[i, j] == -1)				// если срытая ячейка пуста, то есть отсутствует мина
			        {
				        showGrid[i, j] = 0;				// то отображаемая ячейка становится пустышкой
				        grid[i, j] = 0;					// для скрытой ячейки отмечаем как "0 мин вокруг данной мины"
			        }
		        }
	        }

			Uncover();				// раскрываем пустые ячейки вокруг которых 0 мин

			// проверка количества раскрытых ячеек кроме мин
	        int countCells = sizeField.X * sizeField.Y - countMines;	// количество нераскрытых ячеек
	        for (int i = 1; i <= sizeField.X; i++)
	        {
		        for (int j = 1; j <= sizeField.Y; j++)
		        {
			        if (showGrid[i, j] != 10 && showGrid[i, j] != 11) // если текущая ячейка не заглушка и не флаг
			        {
				        countCells--;
			        }
		        }
	        }

	        if (countCells == 0)	// если все ячейки раскрыты кроме мин
		        IsGameWin = true;	// то мы выиграли

	        states.Transform *= Transform;

	        for (int i = 1; i <= sizeField.X; i++)
	        {
		        for (int j = 1; j <= sizeField.Y; j++)
		        {
			        if (showGrid[tile.X, tile.Y] == 9 && grid[i, j] == 9) // если нажатая и скрытая ячейка является миной
			        {
				        showGrid[i, j] = grid[i, j];			// раскрываем все мины
						IsGameOver = true;						// и ставится проигрыш
			        }

					sprite.TextureRect = new IntRect(showGrid[i,j]*width, 0 , width, width);	// устанавливаем рамку текстуры для спрайта
                    sprite.Position = new Vector2f(i * width - width, j * width - width);		// устанавливаем позицию отрисовки
					target.Draw(sprite, states);												// отрисовываем спрайт в заданную цель
                }
	        }
        }

        public void Reset()
        {
            Init();
            IsGameOver = false;
            IsGameWin = false;
            countFlags = 0;
        }		// сброс

        private void Init()
        {
            //заполнение видимого игрового поля
            for (int i = 1; i <= sizeField.X; i++)
            {
                for (int j = 1; j <= sizeField.Y; j++)
                {
                    showGrid[i, j] = 10;
                    grid[i, j] = 0;
                }
            }

            //генерация мин
            int count = countMines;                             // копируем количество мин
            do
            {
                // генерируем случайно координату для установки мины
                int i = Program.rand.Next(1, sizeField.X);
                int j = Program.rand.Next(1, sizeField.Y);

                if (grid[i, j] != 9)        // если текущая ячейка не мина
                {
                    grid[i, j] = 9;         // утанавливаем мину
                    count--;                // уменьшаем количество неутановленных мин
                }

            } while (count != 0);           // расставляем мины пока не все не закночатся

            //подсчёт мин вокруг каждой ячейки
            for (int i = 1; i <= sizeField.X; i++)
            {
                for (int j = 1; j <= sizeField.Y; j++)
                {
                    int number = 0;
                    if (grid[i, j] == 9) continue;
                    if (grid[i + 1, j] == 9) number++;
                    if (grid[i, j + 1] == 9) number++;
                    if (grid[i - 1, j] == 9) number++;
                    if (grid[i, j - 1] == 9) number++;
                    if (grid[i + 1, j + 1] == 9) number++;
                    if (grid[i - 1, j - 1] == 9) number++;
                    if (grid[i - 1, j + 1] == 9) number++;
                    if (grid[i + 1, j - 1] == 9) number++;
                    grid[i, j] = number;
                }
            }

        }	   // инициализирует поле

        private void Fill(int x, int y)
        {
	        if (showGrid[x, y] == 11)		// если видимая ячейка флаг
		        countFlags--;				// то возвращаем его

	        if (grid[x, y] == 0) grid[x, y] = -1;		// если вокруг ячейки мины отсутствуют, то есть их количество равно 0, то помечаем как -1 
            if (grid[x - 1, y] == 0) Fill(x - 1, y);
            if (grid[x + 1, y] == 0) Fill(x + 1, y);
            if (grid[x, y - 1] == 0) Fill(x, y - 1);
            if (grid[x, y + 1] == 0) Fill(x, y + 1);
        }		// рекурсивный метод, очищающий путные ячейки

        private void Uncover()
        {
            for (int i = 1; i <= sizeField.X; i++)
            {
                for (int j = 1; j <= sizeField.Y; j++)
                {
                    if (showGrid[i, j] == 0)		// если отображаемая ячейка является пустой
                    {
						// если текущая ячейка флаг, то его возвращаем
                        if (showGrid[i - 1, j - 1]		== 11) countFlags--;
                        if (showGrid[i, j - 1]			== 11) countFlags--;
                        if (showGrid[i + 1, j - 1]		== 11) countFlags--;
                        if (showGrid[i + 1, j]			== 11) countFlags--;
                        if (showGrid[i + 1, j + 1]		== 11) countFlags--;
                        if (showGrid[i, j + 1]			== 11) countFlags--;
                        if (showGrid[i - 1, j + 1]		== 11) countFlags--;
                        if (showGrid[i - 1, j]			== 11) countFlags--;

                        // отображаем все ячейки вокруг пустой

                        showGrid[i - 1, j - 1]	= grid[i - 1, j - 1];
                        showGrid[i, j - 1]		= grid[i, j - 1];
                        showGrid[i + 1, j - 1]	= grid[i + 1, j - 1];
                        showGrid[i + 1, j]		= grid[i + 1, j];
                        showGrid[i + 1, j + 1]	= grid[i + 1, j + 1];
                        showGrid[i, j + 1]		= grid[i, j + 1];
                        showGrid[i - 1, j + 1]	= grid[i - 1, j + 1];
                        showGrid[i - 1, j]		= grid[i - 1, j];
                    }
                }
            }
        }				// раскрывает ячейки вокруг пустой
    }
}
