using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Button : UI_Popup
{
    //�ó�����
    //componenet�� �̸����� Text object��ü�� ������
    //���� Ÿ�Գ��� ����
    
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

        //itemIcon ���� ������Ʈ 
       // GameObject go = GetImage((int)Images.itemIcon).gameObject;

        //itemIcon ���콺�� �Բ� �̵�
      //  AddUIEvent(go, ((PointerEventData eventData) => { go.transform.position = eventData.position; }), Define.UIEvent.Drag);


    }
    private void Update()
    {
        
    }
    int _score = 0;

    public void OnButtonClicked(PointerEventData eventData)
    {
        _score++;
        GetText((int)Texts.scoreText).text = $"���� : {_score}";
    }

}
