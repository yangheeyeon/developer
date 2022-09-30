using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Button : UI_Popup
{
    //시나리오
    //componenet의 이름으로 Text object자체를 가져옴
    //같은 타입끼리 정리
    
    enum Buttons
    {
        pointerButton,
    }
    enum Texts
    {
        pointerText,
        scoreText,
    }
    enum GameObjects
    {
        testObject,
    }
    enum Images
    {
        itemIcon,
    }
    private void Start()
    {
        
        Init();

    }
    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));

       // Bind<Image>(typeof(Images));
       // Bind<GameObject>(typeof(GameObjects));

        GetButton((int)Buttons.pointerButton).gameObject.AddUIEvent(OnButtonClicked, Define.UIEvent.Click);

        //itemIcon 게임 오브젝트 
       // GameObject go = GetImage((int)Images.itemIcon).gameObject;

        //itemIcon 마우스와 함께 이동
      //  AddUIEvent(go, ((PointerEventData eventData) => { go.transform.position = eventData.position; }), Define.UIEvent.Drag);


    }
    private void Update()
    {
        
    }
    int _score = 0;

    public void OnButtonClicked(PointerEventData eventData)
    {
        _score++;
        GetText((int)Texts.scoreText).text = $"점수 : {_score}";
    }

}
