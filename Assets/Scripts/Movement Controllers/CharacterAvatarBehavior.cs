using UnityEngine;
using System.Collections;

public class CharacterAvatarBehavior : MonoBehaviour {
    Transform connectionTarget;

    [SerializeField]
    public AvatarParts parts;

    public void SetAvatarShape(CharacterMovementInfo inputCharacterMovementInfo)
    {

    }

    public void ConnectTo(Transform inputConnectionTransform)
    {
        connectionTarget = inputConnectionTransform;
        if (connectionTarget.GetComponent<CharacterMovementController>())
        {
            CharacterMovementInfo c = connectionTarget.GetComponent<CharacterMovementController>().characterInfo;
            parts.head.localScale = parts.head.localScale * ((float) c.movementSpeed / 700f);
            parts.torso.localScale = parts.torso.localScale * ((float)c.weight / 2f);
        }
    }

    public void Disconnect()
    {
        connectionTarget = null;
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        if (connectionTarget)
        {
            transform.position = connectionTarget.position + Vector3.up;
        }
    }

    [System.Serializable]
    public class AvatarParts
    {
        public Transform chest;
        public Transform head;
        public Transform torso;
    }
}
