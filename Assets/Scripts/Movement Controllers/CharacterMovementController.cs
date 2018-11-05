using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterMovementController : MonoBehaviour {
    public enum ControllerTypes { player, computer }
    public ControllerTypes controlledBy = ControllerTypes.player;
    public Rigidbody controllerRigidbody;
    
    [SerializeField]
    public CharacterMovementInfo characterInfo = new CharacterMovementInfo();
    Vector2 startPointerPosition = new Vector2(0,0);

    #region Avatar Information
    public GameObject characterBodyPrefab;
    GameObject characterBody;
    CharacterAvatarBehavior characterAvatar;
    #endregion

    #region AI Variables
    Vector3 currentTargetLocation;
    Transform currentTargetTransform;
    #endregion

    void Start()
    {
        GamePhaseBehavior_Play.OnCharacterUpdate += MovementBehavior;

        characterInfo.movementSpeed = Random.Range(700,900);
        characterInfo.weight = Random.Range(1, 4);

        if (characterBody == null)
        {
            characterBody = (GameObject) Instantiate(characterBodyPrefab, transform.position + Vector3.up, transform.rotation);
            characterAvatar = characterBody.GetComponent<CharacterAvatarBehavior>();
            characterAvatar.ConnectTo(this.transform);
        }
    }

    void FixedUpdate()
    {
    //    MovementBehavior();
    }

    void MovementBehavior()
    {
        switch (controlledBy)
        {
            case ControllerTypes.player:
                if (Input.GetMouseButtonDown(0))
                {
                    startPointerPosition = InputManager.instance.GetPointerPosition();
                }
                else if (Input.GetMouseButton(0))
                {
                    Vector2 pointerPosition = InputManager.instance.GetPointerPosition();
                    float analogStickMagnitude = InputManager.instance.FetchAnalogStickMagnitude();
                    Vector2 movementFactor = (pointerPosition - startPointerPosition).normalized;
                    controllerRigidbody.AddForce(new Vector3(movementFactor.x, 0, movementFactor.y) * characterInfo.movementSpeed * Time.deltaTime);
                    characterBody.transform.rotation = Quaternion.LookRotation(new Vector3(movementFactor.x, 0, movementFactor.y));

                    Debug.Log("Movement Vector is: " + movementFactor);
                    //characterBall_Rigidbody.AddTorque(controllerRigidbody.velocity.normalized * 10f);
                }
                else if (Input.GetMouseButtonUp(0))
                {
                }
                break;
            case ControllerTypes.computer:
                if (currentTargetTransform)
                {
                    Vector3 movementVector = (currentTargetTransform.position - transform.position).normalized;
                    controllerRigidbody.AddForce(movementVector * characterInfo.movementSpeed * Time.deltaTime);
                    characterBody.transform.rotation = Quaternion.LookRotation(new Vector3(movementVector.x, 0, movementVector.z));

                }
                else
                {
                    currentTargetTransform = GameManager.instance.FetchOtherPlayer(this.gameObject);
                }
                break;
        }
    }

    public void EndCharacterMovement()
    {
        GamePhaseBehavior_Play.OnCharacterUpdate -= MovementBehavior;
        GetComponent<Collider>().enabled = false;
        controllerRigidbody.velocity = Vector3.zero;
        controllerRigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }

    void PlayDeathAnimation() {
        controllerRigidbody.AddExplosionForce(500f, Vector3.zero + Vector3.down * 10f, 100f);
        controllerRigidbody.constraints = RigidbodyConstraints.None;
        controllerRigidbody.AddTorque(new Vector3(Random.Range(50f, 100f), Random.Range(-40f, 40f), Random.Range(50f, 100f)));

        if (characterBody)
        {
            if (characterAvatar)
            {
                characterAvatar.Disconnect();
            }
            characterBody.AddComponent<Rigidbody>().AddExplosionForce(500f, Vector3.zero + Vector3.down * 10f, 100f);
            characterBody.GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(50f, 100f), Random.Range(-40f, 40f), Random.Range(50f, 100f)));
        }
    }

    public void Die()
    {
        PlayDeathAnimation();
        Destroy(gameObject, 5f);
    }

    public void QuickDie()
    {
        if (characterAvatar)
        {
            Destroy(characterAvatar.gameObject);
        }
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.tag == "Player")
        {
            Vector3 bounceDirection = (transform.position - c.gameObject.transform.position).normalized;
            bounceDirection += Vector3.up * 0.5f;
            controllerRigidbody.velocity = bounceDirection * 3f;

            if (controlledBy == ControllerTypes.computer)
            {
                currentTargetTransform = null;
            }
        }
        else if (c.gameObject.tag == "Bounds")
        {
            EndCharacterMovement();
            GameManager.instance.ReportPlayerDeath(this);
        }
    }

    #region Computer Behaviors
    public GameObject ReturnRandomCharacter()
    {
        GameObject returnObject = null;

        return returnObject;

    }
    #endregion
}

[System.Serializable]
public class CharacterMovementInfo
{
    public int playerNumber = 0;
    public float movementSpeed = 5f;
    public float rotationSpeed = 5f;
    public float weight = 5f;
}
