using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�ڷ�ƾ ?
//������ �Ͻ� �����ϰ� ���� �����ӿ��� �ߴ��ߴ� ��ġ���� �ٽ� �̾�� �� �ִ� �Լ�!!
public class GameScene : BaseScene
{
    
    protected override void Init()
    {
        //override�� Init()�� ȣ��, GameScene���� baseSceneȣ��
        base.Init();
        
        //scene�� �������� �����
        SceneType = Define.Scene.Game;
        Managers.UI.showSceneUI<UI_Inven>();
        gameObject.GetOrAddComponent<CursorController>();

        GameObject player = Managers.Game.Spawn(Define.WorldObject.Player, "UnityChan");
        Camera.main.gameObject.GetOrAddComponent<CameraController>().SetPlayer = player;
        

        GameObject go = new GameObject("@SpawningPool");
        SpawningPool pool = go.GetOrAddComponent<SpawningPool>();
        //��ü ���� �� ����
        pool.SetKeepMonsterCount(5);
    }
    
    
    public override void Clear()
    {
    }

}
