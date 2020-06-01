using UnityEngine;

using STP.Utils;

public class FollowCamera : GameBehaviour {
    public Camera        Camera;
    public Transform     CameraTransform;
    public RectTransform Player;
    public RectTransform Background;

    protected override void CheckDescription() =>
        ProblemChecker.LogErrorIfNullOrEmpty(this, Camera, Player, Background);

    void Start() {
        Camera.orthographicSize = Screen.height / 2.0f;
    }

    void Update() {
        CameraTransform.position = GetPlayerPosition();
    }

    Vector3 GetPlayerPosition() => new Vector3(Player.position.x, Player.position.y, -10f);
}
