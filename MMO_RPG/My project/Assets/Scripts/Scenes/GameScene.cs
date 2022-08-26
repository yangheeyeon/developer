using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//코루틴 ?
//실행을 일시 정지하고 다음 프레임에서 중단했던 위치에서 다시 이어나갈 수 있는 함수!!
public class GameScene : BaseScene
{
    
    protected override void Init()
    {
        //override한 Init()만 호출, GameScene에서 baseScene호출
        base.Init();
        
        //scene의 선봉대장 만들기
        SceneType = Define.Scene.Game;
        Managers.UI.showSceneUI<UI_Inven>();
        gameObject.GetOrAddComponent<CursorController>();

        GameObject player = Managers.Game.Spawn(Define.WorldObject.Player, "UnityChan");
        Camera.main.gameObject.GetOrAddComponent<CameraController>().SetPlayer = player;
        

        GameObject go = new GameObject("@SpawningPool");
        SpawningPool pool = go.GetOrAddComponent<SpawningPool>();
        //전체 몬스터 수 세팅
        pool.SetKeepMonsterCount(5);
    }
    
    
    public override void Clear()
    {
    }

}
