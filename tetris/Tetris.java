package tetris;

import java.awt.*;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.util.Timer;
import java.util.TimerTask;

import javax.swing.JButton;
import javax.swing.JDialog;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JOptionPane;



public class Tetris extends JFrame {
	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	//게임 상태 표시 
    private JLabel statusbar;
    
    
    //테트리스 생성자
    public Tetris() {

        initUI();
    }
    
    private void initUI() {
    	
    

	    statusbar = new JLabel(" 0");
	     
	    add(statusbar, BorderLayout.SOUTH);
        
        Board board =  new Board(this);
        
        
        
        add(board); //JFrame에 추가
        
    	//게임 상태바 
	        
	    
        
        
        board.start();
        
        setTitle("재밌는 테트리스 게임");
        
        setSize(300,500);
        
        setDefaultCloseOperation(EXIT_ON_CLOSE);
        
        setLocationRelativeTo(null);
        
        setResizable(false);//GUI 크기 고정
        
        
        
    } 
    
	public JLabel getStatusbar() {
		
		return statusbar;
	}
    
    
    public static void main(String[] args) {
    	
    	EventQueue.invokeLater(()->{
    		
    		Tetris game = new Tetris();
    		
    		game.setVisible(true);
    		
    	
    			
    	});
    	
    }


}
