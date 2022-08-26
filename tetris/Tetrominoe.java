package tetris;


import java.util.Random;

public class Tetrominoe {

	//전역 변수
	protected enum Shape {
		NoShape , ZShape, SShape, LineShape, FlipedLineShape,
		TShape, SquareShape, MirroredLShape, LShape
	}
	
	private Shape pieceShape;
	private int coords[][];
	private int[][][] coordsTable;
	
	
	//생성자
	public Tetrominoe() {
		
		initShape();
		
	}
	
	

	//메서드
	private void initShape() {
		//지역변수 선언
	
		// 4 *2 행렬
		coords = new int[4][2];
		
		//9개 테트리스 모양
		//4개의 정사각형 외쪽 꼭짓점 좌표 
		coordsTable = new int[][][] {
			
		
			{ { 0 , 0 }  , { 0 , 0 } , { 0 , 0 } , { 0 , 0 }   },
			{ { -1 , 1 } , { -1 , 0 }, { 0, 0 }  , { 0 , -1 }  },
			{ { -1 , 0 } , { 0 , 0 } , { 1 , 0 } , { 1 , 1 }   },
			{ { 0 , 2 }  , { 0 , 1 } , { 0 , 0 } , { 0 , -1 }  },//FlipedLineshape
			{ { 0 , 2 }  , { 0 , 1 } , { 0 , 0 } , { 0 , -1 }  },
			{ { -1 , 0 } , { 0 , 0 } , { 1 , 0 } , { 0 , 1 }   },
			{ { 0 , 0 }  , { 1 , 0 } , { 0 ,  1 } , { 1 , 1}   },
			{ { 0 , 1 }  , { 0 , 0 } , { 0 , -1 } , { 1 , -1 } },
			{ { 0 , 1 }  , { 0 , 0 } , { 0 , -1 } , { -1 , -1 }}
	

			
		
		};
		
			setShape(Shape.NoShape);
		
			

	
	}//initShape 메서드 

	//랜덤으로 테트리스 블럭 모양을 만드는 메서드
	public void setShape(Shape shape) {  
		
		
		for (int i = 0; i < 4; i++) {
			
			for (int j = 0; j <2 ; j++) {
				
				coords[i][j] = coordsTable[shape.ordinal()][i][j];
				
			}
		}
		pieceShape = shape;
		
	}
	public Shape getShape() {
		
		return pieceShape;
	}
	
	
	public void setX(int index, int x) {
		
		coords[index][0] = x;

	}
	public void setY(int index, int y) {
		
		coords[index][1] = y;
		
	}
	public int getX(int index) {
		
		
		return coords[index][0];
		
	}
	public int getY(int index) {
		
		return coords[index][1];
		
	}
	
	public void setRandomShape() {
		Random r = new Random();
		
		int x  = Math.abs(r.nextInt()) % 8 + 1;// x번째 열거체 상수를 랜덤하게 고름
		
		Shape[] values = Shape.values();
		setShape(values[x]);//열거체 상수를 매개변수로 넘겨서 테트리스 생성함
		
	}
	
	public int minX() {
		
		int m = coords[0][0];
		
		for (int i = 0; i < 4 ; i++ ) {
			
			if ( coords[i][0] < m ) {
				m = coords[i][0];
			}
			
		}
		return m;
		
	}
	
	public int minY() {
		
		int m = coords[0][1];
		
		for (int i = 0; i< 4 ; i++) {
			
			if( coords[i][1] < m ) {
				m = coords[i][1];
			}
		}
		return m;
		
		
	}
	//반시계 방향으로 90도 회전 시키기
	public Tetrominoe rotateLeft() {
		
		if (pieceShape == Shape.SquareShape) {
			
			return this;
			
		}
		
		Tetrominoe result = new Tetrominoe();
		
		result.pieceShape = pieceShape;
		
		for (int i = 0; i< 4; i++) {
			
			
			result.setX(i, -1*getY(i) );//열과 행 기준 상대 인덱스
			result.setY(i, getX(i) );
			
		}
		
		return result;
	}
	
	
	//시계방향으로 회전 
	public Tetrominoe rotateRight() {
		
		if (pieceShape == Shape.SquareShape) {
			
			return this;
		}
		
		Tetrominoe result = new Tetrominoe();
		
		result.pieceShape = pieceShape;
		
		for (int i = 0; i< 4; i++) {
			
			
			result.setX(i, getY(i) );
			result.setY(i, -1*getX(i) );
			
			
		}
		return result ;
		
		
	}



	
	
}//  외부 클래스 
