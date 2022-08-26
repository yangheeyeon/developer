package tetris;

import java.awt.BorderLayout;
import java.awt.Color;
import java.awt.Graphics;
import java.awt.Image;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.awt.event.KeyEvent;
import java.awt.event.KeyListener;

import javax.swing.ImageIcon;
import javax.swing.JButton;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JPanel;
import javax.swing.Timer;



import tetris.Tetrominoe.Shape;

public class Board extends JPanel{

		/**
	 * 
	 */
		private static final long serialVersionUID = 1L;
		
		
		//오버라이딩
		@Override
		protected void paintComponent(Graphics g) {
			
			
			super.paintComponent(g);
			doDrawing(g);
			
			
		}
		
		
		private final int BOARD_WIDTH = 10; //열 개수
		private final int BOARD_HEIGHT = 22; //행 개수
		private final int PERIOD_INTERVAL = 300 ;//0.3초 마다 반복
		private boolean isPaused;
		private Timer timer;
		private int numLinesRemoved = 0;//삭제된 줄 개수
		private int numFullLines = 0;
		private int curX = 0;// 내려오는 테트리스의 열 인덱스
		private int curY = 0;// 내려오는 테트리스의 행 인덱스
		private JLabel statusbar;
		private Tetrominoe curPiece ;
		private Shape[] board;
		private boolean isFallingFinished = false;
		
		
		//생성자
		public Board(Tetris tetris) {
			
	
			initBoard(tetris);
			
		}
		
		//**새롭게 추가한 메서드 
		//밑에서 세줄 까지 빈 공간을 채워주는 메서드
		
		public void helper() {
		
			
			//위에 있는 행을 아래로 내리기
			for (int k = BOARD_HEIGHT-1; k > 0; k--) {
				
				for (int j = 0 ;  j < BOARD_WIDTH ; j++) {
					
					board[ k*BOARD_WIDTH + j ] = shapeAt( j , k-1 );
				}
				
			}
			//맨윗줄은 직접 처리하기
			for (int j =0; j <BOARD_WIDTH; j++) {
				
				board[j] = Shape.NoShape;
				
			}
			
			repaint();
			}
	
		
		//초기화 메서드
		private void initBoard(Tetris tetris) {
			

			setFocusable(true);//키보드 입력이 이곳에 모두 들어갈 수 있도록 함
			
			statusbar = tetris.getStatusbar();//게임 상태를 가져옴
			
			addKeyListener(new KAdapter());//키보드 입력을 어떻게 처리할지 메서드 오버라이딩 해야함
			
		}
		
		
		
		
		//블럭 한개의 가로길이
		private int squareWidth() {
			
			return (int) getSize().getWidth() / BOARD_WIDTH ;
			
		}
		//블럭 한개의 세로길이
		private int squareHeight() {
			
			return (int) getSize().getHeight() / BOARD_HEIGHT;
			
		}
		//해당하는 행과 열에 어떤 모양이 들어가는지 알려는 메서드
		private Shape shapeAt(int x , int y ) {
			
			return board[ y * BOARD_WIDTH + x];
			
		}
	    void start() {

	    	//맨처음 시작할때 만드는 curPiece
	        curPiece = new Tetrominoe();
	        
	        
	        //열 개수 * 행 개수
	        board = new Shape[BOARD_WIDTH * BOARD_HEIGHT];
	        
	        timer = new Timer(PERIOD_INTERVAL, new GameCycle());
	        
	        timer.start();
	        
	        //모든 열과 행의 shape 를 NoShape로 바꾸기
	        clearBoard();
	        
	        
	        newPiece();

	        
	    }
		//멈추면 화면 재구성 하는 메서드
		private void pause() {
			isPaused = !isPaused;
			
			if(isPaused) {
				
				statusbar.setText("Paused");
				
			}else {
				
				statusbar.setText(String.format("score : %d", numLinesRemoved ));
				
			}
			repaint();
			
		}
	
		
		
		private void doDrawing(Graphics g) {
			//배경 이미지 삽입
			Image background = new ImageIcon("src/images/background.png").getImage();
			g.drawImage(background, 0, 0 ,null);
			
			var size = getSize(); //자바에서 var는 자료형을 명시하지 않는 경우에 쓰임
			
			int boardTop = (int) size.getHeight() - BOARD_HEIGHT * squareHeight();// 전체 높이 - 행 개수 x  세로 길이
			
			
			
			for (int i = 0; i < BOARD_HEIGHT ; i++) {//행 개수 만큼 회전
				
				
				for (int j = 0; j< BOARD_WIDTH ; j++) {//열 개수 만큼 회전
					
					Shape shape = shapeAt( j , i );//열과 행
					
					if (shape != Shape.NoShape) {
						
						//실제 (0,0) 좌표 기준 떨어진 거리 
						drawSquare( g , j *squareWidth() , boardTop + i *squareHeight() , shape);
						
					}
					
					
					
				}
			}//for문 종료
			
			if (curPiece.getShape() != Shape.NoShape) {
				
				for (int i = 0; i <4 ; i++) {
					//square 하나 하나의 열, 행 인덱스 
					int x = curX + curPiece.getX(i);
					int y = curY - curPiece.getY(i);
					
					drawSquare(g , x * squareWidth() , y * squareHeight() + boardTop, curPiece.getShape());
					
				}
			}
			
		
			
			repaint();
			
		}
		//space bar를 누르면 실행되는 메서드
		private void dropDown() {
			
			int newY = curY;
			
			while( newY < BOARD_HEIGHT ) {
				//행 번호 늘리기
				if (! tryMove(curPiece, curX ,newY + 1 ) ) {
					break;//더이상 아래로 내려 가지 않고 반복문 빠져 나옴
					
				}
				newY++;
			}
			pieceDropped();
		}
		
		//업데이트 메서드에서 계속 호출됨
		private void oneLineDown() {
			//행 번호 증가시킴
			if( ! tryMove(curPiece, curX, curY + 1)) {
				
				pieceDropped();
				
			}
		}
		
		private void clearBoard() {
			
			
			for (int i =0 ; i < BOARD_WIDTH * BOARD_HEIGHT ; i++) {
				
				board[i] = Shape.NoShape;
				
			}
				
				
				
		}
		//블럭이 더이상 내려가지 못할때 실행되는 메서드
		private void pieceDropped() {
			
			for (int i = 0 ; i < 4 ; i++) {
				
				int x = curX + curPiece.getX(i);// 각각 블럭 왼쪽 꼭짓점의 열
				int y = curY - curPiece.getY(i);// 각각블럭 왼쪽 꼭짓점의 행
				
				board[ y * BOARD_WIDTH + x] = curPiece.getShape();
				
				
				
			}
			
			//한 줄 가득 차면 버리기
			removeFullLines();
			
			isFallingFinished = true ;//true 이면 새로운 테트리스
			
			curPiece.setShape(Shape.NoShape);
			
			
		}
		
		private void newPiece() {
			
			if (curPiece.getShape() == Shape.NoShape ) {
				
				curPiece.setRandomShape();
				
				curX = BOARD_WIDTH /2 + 1;//처음 열 인덱스 
			
				curY = 2;	//처음 행 인덱스
				
			}
		
			
			if (! tryMove(curPiece, curX, curY)) {
			
				curPiece.setShape(Shape.NoShape);
				
				timer.stop();
				
				String msg = String.format("게임 종료 score = %d ", numLinesRemoved);
				
				statusbar.setText(msg);
				
				
				
				}

				
				
			}
		//해당 열과 행으로 이동할 수 있는지 알아봄
		
		private boolean tryMove(Tetrominoe newPiece, int newX, int newY ) {
			
			for (int i = 0; i <4 ; i++) {
				
				int x = newX + newPiece.getX(i);//현재 열 인덱스에서 블럭 좌표 더하기
				int y = newY - newPiece.getY(i); //현재 행 인덱스에서 블럭 좌표 더하기
				
				//최대 열, 행 인덱스를 넘어가면 false
				if( x < 0 || x >= BOARD_WIDTH || y < 0 || y >= BOARD_HEIGHT) {
					
					return false;
				}
				//이미 그 열과 행에 다른 테트리스가 있으면 false
				if (shapeAt(x ,y) != Shape.NoShape) {
					
					return false;
				}
				
		
			}//for문 종료
			
			curPiece = newPiece;
			curX = newX;
			curY = newY;
			
			return true;
					
			
			
			
		}
		
	//블럭이 더이상 내려가지 못할떄 실행되는 메서드
		private void removeFullLines() {
			try {
			int FullLines = 0;
			
			for (int i = BOARD_HEIGHT-1  ; i>0 ; i--) { //바닥에서 부터 확인
				
				boolean lineIsFull = true;
				
				for (int j = BOARD_WIDTH-1 ; j >=0; j--) {
					
					if ( shapeAt(j,i) == Shape.NoShape ) {//행과 열에 NoShape이 있으면
						
						lineIsFull = false; 
						
						break; 
						
						}
					
					
				}
				 
				if( lineIsFull ) {
					
					FullLines++;
					
					//위에 있는 행을 아래로 내리기
					for (int k = i; k > 0; k--) {
						
						for (int j = 0 ;  j < BOARD_WIDTH ; j++) {
							
							board[ k*BOARD_WIDTH + j ] = shapeAt( j , k-1 );
						}
						
					}
					//맨윗줄은 직접 처리하기
					for (int j =0; j <BOARD_WIDTH; j++) {
						
						board[j] = Shape.NoShape;
						
					}
					
				}//한줄씩 내리는 블록 종료
				
				
				
		}//for문 종료
			
			
			
			if (FullLines > 0) {
				
				numLinesRemoved += FullLines;
				
				statusbar.setText(String.valueOf(numLinesRemoved));
				
				

			}
			
			}catch(ArrayIndexOutOfBoundsException e) {
				System.out.println(e.toString());
			}
		
		}
		
	
		//테트리스를 구성하는 정사각형 하나 하나를 그리는 메서드
		
		//실제 x y좌표
		private void drawSquare(Graphics g, int x, int y, Shape shape) {
				
			Color colors[] = {new Color(0, 0, 0), new Color(204, 102, 102),
	                new Color(102, 204, 102), new Color(255,174,215), new Color(102, 102, 204),
	                new Color(204, 204, 102), new Color(204, 102, 204),
	                new Color(102, 204, 204), new Color(218, 170, 0)
	        };
			
			var color = colors[shape.ordinal()];//shape가 열거체에서 위치한 순서 이용
			
			g.setColor(color);
			
			//테투리 1씩 남겨두고 칠하기
			//fillRect(left_x ,top_y ,width, height)
			
			g.fillRect(x+1, y+1, squareWidth() -2, squareHeight() -2);
			
			g.setColor(color.brighter());
			g.drawLine(x, y + squareHeight() -1 , x, y);
			g.drawLine(x + squareWidth() -1 , y, x, y);
			
			g.setColor(color.darker());
			g.drawLine(x+1, y+squareHeight()-1, x+squareWidth()-1, y+squareHeight()-1);
			g.drawLine(x + squareWidth()-1, y, x+squareWidth()-1, y+squareHeight()-1);
			
			

			
			
			
		}    
		private class GameCycle implements ActionListener{

			@Override
			public void actionPerformed(ActionEvent e) {
				doGameCycle();
			}

			private void doGameCycle() {
				
				update();
				repaint();
				
			}
			
			
		}
		public void update() {
			
			
			if(isPaused) {
				
				return;
						
						
			}//만약 테트리스가 떨어지는게 끝나면
			//새로운 테트리스 떨어뜨리기
			if (isFallingFinished) {
				
				isFallingFinished = false;
				
				newPiece();
				
			}
			else {
				oneLineDown();
			}
		
		}//abstract class KeyAdapter -> implements KeyListener interface
		class KAdapter implements KeyListener{

			@Override
			public void keyTyped(KeyEvent e) {
				
			}

			@Override
			public void keyPressed(KeyEvent e) {
				
				if (curPiece.getShape() == Shape.NoShape) {
					return;
				}
				
				int keyCode = e.getKeyCode();
				//개선된 switch 문 java14 ~
				switch(keyCode) {
					
				case KeyEvent.VK_P ->pause();
				case KeyEvent.VK_LEFT -> tryMove(curPiece,curX -1,curY);
				case KeyEvent.VK_RIGHT -> tryMove(curPiece,curX + 1,curY);
				case KeyEvent.VK_DOWN -> tryMove(curPiece.rotateRight(),curX,curY);
				case KeyEvent.VK_UP -> tryMove(curPiece.rotateLeft(),curX,curY);
				case KeyEvent.VK_SPACE -> dropDown();
				case KeyEvent.VK_D -> oneLineDown();
				case KeyEvent.VK_CAPS_LOCK-> helper(); 
				
			
				
				}//switch 종료
				
				
				}

			@Override
			public void keyReleased(KeyEvent e) {
				
				
			}
			
		
		}
		
		
	
	
	
	
	
}
