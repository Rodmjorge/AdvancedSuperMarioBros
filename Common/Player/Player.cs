using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    private Animator animator;

    private void Start() {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        rigidBody.gravityScale = LevelSettings.GetGravity();
        transform.position = LevelSettings.ShiftToPosition(transform, 0f);
    }

    private void Update() {
        rigidBody.velocity = GravitySettings.MaxVelocity(rigidBody, 3f);

        if (LevelSettings.playerDied || LevelSettings.stopEverything) {
            FreezePlayer();
        }
        else {
            rigidBody.isKinematic = false;
        }

        if (Input.GetKey(KeyCode.R)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            LevelSettings.ResetSettings();
        }
    }

    private void OnTriggerEnter2D(Collider2D collider) {

        if (collider.gameObject.tag == "Enemy") {
            Enemy enemy = Enemy.GetEnemy(collider.gameObject);

            enemy.playerHit = true;
            enemy.playerThatHit = gameObject;
        }
        else if (collider.gameObject.tag == "Block") {
            Block block = Block.GetBlock(collider.gameObject);

            block.playerCollided = true;
            block.playerThatCollided = gameObject;
        }
        else if (collider.gameObject.tag == "Object") {
            ObjectM objectM = ObjectM.GetObject(collider.gameObject);

            objectM.playerCollided = true;
            objectM.playerThatCollided = gameObject;
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        OnTriggerEnter2D(collision);
    }

    public void PlayerGotHit() {
        animator.Play("Death");
        LevelSettings.playerDied = true;
    }

    public void FreezePlayer() {
        rigidBody.isKinematic = true;
    }

    public static PlayerMovement GetPlayerMovement(GameObject player) {
        return player.GetComponent<PlayerMovement>();
    }

    public static Player GetPlayer(GameObject player) {
        return player.GetComponent<Player>();
    }
}
