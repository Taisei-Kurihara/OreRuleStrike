using UnityEngine;
using UniRx;
using System;


/// <summary>
/// ショット判定等を行える。基本的なモード
/// </summary>
public class GameMode_MainMode : IGameMode
{
    GameSessionManager GameSceneManager;

    PlayerSessionData player;

    #region カメラコンポーネント
    /// <summary>
    /// 中心
    /// </summary>
    public Vector3 Cont = new Vector3(0, 0, 0);
    public float Dist = 3f;
    #endregion

    public ReactiveProperty<bool> dragged = new ReactiveProperty<bool>(false);

    private Vector3 position;
    private Rigidbody rb;
    private Vector3 dragOffset;

    private Vector3 direct;

    public GameMode_MainMode(GameSessionManager gameSceneManager)
    {
        GameSceneManager = gameSceneManager;
    }

    /// <summary>
    /// 変更する数値
    /// </summary>
    public float ChangeMeter = 0;
    IDisposable dis_dragg;

    private bool ShotAfter = false;

    /// <summary>
    /// ここで次のプレイヤーの情報を入れる
    /// </summary>
    void IGameMode.Init()
    {
        player = GameSceneManager.NowPlayer();

        _ = player.WaitForCardUI(player.UI_Set_Main);

        //盤面上に存在しない場合。
        if (player.Player_GamePiece == null)
        {
            player.PlayerPieceCreate();
        }

        //現在のキャラクターの地点を取得する
        Cont = player.Player_GamePiece.transform.position;
        Cont.y = 0;

        rb = player.Player_GamePiece.GetComponent<Rigidbody>();

        //ドラッグする為の宣言
        dis_dragg = dragged.Where(_ => _ == true).Subscribe(_ => {
            position = Input.mousePosition;
        }).AddTo(player.Player_GamePiece);
    }

    /// <summary>
    /// ショット等を作成する。
    /// </summary>
    void IGameMode.Update()
    {
        if (player.Player_GamePiece)
        {
            //カメラの宣言
            CameraRole();

            //二回以上ショットできないようにする。
            if (ShotAfter == false)
            {
                //線引きとショットの関数
                ShotLine();
            }
        }
    }

    /// <summary>
    /// ここでカメラの処理を行っているため、現在のプレイヤーさえわかればカメラ処理は基本問題ない。
    /// </summary>
    void CameraRole()
    {
        // 上限値に行けば数値を戻す
        if (ChangeMeter >= 361)
        {
            ChangeMeter = 0;
        }
        if (ChangeMeter < 0)
        {
            ChangeMeter = 360;
        }

        // 数値を変更
        if (Input.GetMouseButton(2))
        {
            ChangeMeter += 0.3f;
        }
        if (Input.GetMouseButton(1))
        {
            ChangeMeter -= 0.3f;
        }

        //現在のキャラクターの地点
        Cont = player.Player_GamePiece.transform.position;
        Cont.y = 0;

        // ラジアン変換
        float test = ChangeMeter * Mathf.Deg2Rad;
        // 移動するべき地点を算出
        float b = Mathf.Cos(test);
        float a = Mathf.Sin(test);
        // 地点作成
        Vector3 t = new Vector3(b * Dist, 3f, a * Dist);
        // カメラの位置を変更
        GameSceneManager.CameraPosition.transform.position = t + Cont;

        // プレイヤーの方向を向かせる
        GameSceneManager.CameraPosition.transform.LookAt(player.Player_GamePiece.transform);

        // 特定のX軸角度を設定（例えば30度）
        float fixedXRotation = 30f;

        // 現在の回転を取得
        Vector3 fixedRotation = GameSceneManager.CameraPosition.transform.rotation.eulerAngles;

        // X軸回転を固定（特定の値に設定）
        fixedRotation.x = fixedXRotation;

        // Y軸とZ軸の回転を維持しつつ、X軸だけ更新
        GameSceneManager.CameraPosition.transform.rotation = Quaternion.Euler(fixedRotation);
    }


    float dis = 0;
    /// <summary>
    /// ここで線とショット判定を行っている。
    /// </summary>
    public void ShotLine()
    {
        Vector3 cameraOffset = player.Player_GamePiece.transform.position - GameSceneManager.CameraPosition.transform.position;

        if (Input.GetMouseButton(0))
        {
            GameSessionManager.Instance().Arrowline.positionCount = 2;
            GameSessionManager.Instance().Arrowline.SetPosition(0, player.Player_GamePiece.transform.position);

            dragged.Value = true;

            // ドラッグ開始からの差分のベクトル
            Vector3 direction = (Input.mousePosition - position) / (new Vector2(Screen.width, Screen.height).magnitude * 0.05f);
            direct = direction;

            dis = ((direction.magnitude / 5) > 1) ? 1 : (direction.magnitude / 5);


            // ラジアン角度を計算
            var rad = Mathf.Atan2(direction.y, direction.x);

            // カメラの方向を基準にしたベクトルを作成
            // カメラの前方向と右方向を取得
            Vector3 cameraForward = GameSceneManager.CameraPosition.transform.forward;
            cameraForward.y = 0; // 垂直方向は無視
            cameraForward.Normalize(); // 正規化

            Vector3 cameraRight = GameSceneManager.CameraPosition.transform.right;
            cameraRight.y = 0; // 垂直方向は無視
            cameraRight.Normalize(); // 正規化

            // カメラの前方と右方向を基にドラッグ方向を設定
            Vector3 dragDirection = cameraForward * Mathf.Sin(rad) + cameraRight * Mathf.Cos(rad);

            // ベクトル作成
            dragOffset = dragDirection;

            // dragOffsetを反転させて反対方向に延ばす
            GameSessionManager.Instance().Arrowline.SetPosition(1, new Vector3(
                player.Player_GamePiece.transform.position.x - (dragOffset.x * ((direction.magnitude > 5) ? 5 : direction.magnitude)),
                1,
                player.Player_GamePiece.transform.position.z - (dragOffset.z * ((direction.magnitude > 5) ? 5 : direction.magnitude))
            ));


        }
        // 離した時
        else if (dragged.Value == true)
        {
            dragged.Value = false;
            GameSessionManager.Instance().Arrowline.positionCount = 0;

            if (direct.magnitude != 0)
            {
                //ショットはイベントを加える。
                ShotAfter = true;
                // 移動量を加える
                rb.AddForce(-(dragOffset * 27) * dis, ForceMode.Impulse);

                //ショット完全終了判定を作る
                player.ShotPoint();
            }
        }
    }



    void IGameMode.FixUpdate()
    {
    }

    void IGameMode.Exit()
    {

    }
}