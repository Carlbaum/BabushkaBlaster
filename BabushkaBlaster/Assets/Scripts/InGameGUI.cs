﻿using UnityEngine;
using System.Collections;

public class InGameGUI : MonoBehaviour {
  //PRIVATE VARIABLES
  private static string guiMode = "InGame";
  private enum gameState { Running, Paused, GameOver, Victorious};
  private gameState state = gameState.Running;


  private static bool buildMode = false;

  private static int selectedTower;
  private static int playerHealth = 9;
  private static int killScore = 0;
  private static int cash = 1337;

  private Material originalMat;

  private GameObject lastHitObj;
  private Ray ray;
  private RaycastHit rayHit;
  private Camera camera;

  // PUBLIC VARIABLES
  public Transform placementGrid;
  public LayerMask placementGridLayer;

  public Material hoverMat;

  public GameObject[] structuresList;

  public float cameraSpeed = 10.0f;
  public float cameraRotSpeed = 50.0f;

  void Awake() {

  }

  void Start() {
    camera = FindObjectOfType<Camera>();
  }

  void Update() {
    switch (state) {
      case gameState.Running:
        if(Input.GetKeyUp(KeyCode.B)) {
          print("PRESSED b-key");
          buildMode = buildMode ? false : true;
        }
        if(Input.GetKeyDown("escape")) {
          print("PRESSED escape-key, PAUSE");
          Time.timeScale = 0;
          state = gameState.Paused;
        }
        if (Input.GetKey(KeyCode.RightArrow)) {
          if (Input.GetKey(KeyCode.LeftAlt)) {
            camera.transform.Rotate(Vector3.up * cameraRotSpeed * Time.deltaTime, Space.World);
          } else {
            camera.transform.Translate(Vector3.right * cameraSpeed * Time.deltaTime, Space.World);
          }
        }
        if (Input.GetKey(KeyCode.LeftArrow)) {
          if (Input.GetKey(KeyCode.LeftAlt)) {
            camera.transform.Rotate(Vector3.down * cameraRotSpeed * Time.deltaTime, Space.World);
          } else {
            camera.transform.Translate(Vector3.left * cameraSpeed * Time.deltaTime, Space.World);
          }
        }
        if (Input.GetKey(KeyCode.DownArrow)) {
          if (Input.GetKey(KeyCode.LeftAlt)) {
            camera.transform.Translate(Vector3.back * cameraSpeed * Time.deltaTime);
          } else {
            camera.transform.Translate(Vector3.back * cameraSpeed * Time.deltaTime, Space.World);
          }
        }
        if (Input.GetKey(KeyCode.UpArrow)) {
          if (Input.GetKey(KeyCode.LeftAlt)) {
            camera.transform.Translate(Vector3.forward * cameraSpeed * Time.deltaTime);
          } else {
            camera.transform.Translate(Vector3.forward * cameraSpeed * Time.deltaTime, Space.World);
          }
        }
        break;
      case gameState.Paused:
        if (Input.GetKeyDown("escape")) {
          //        print ("Resuming game...");
          Time.timeScale = 1;
          state = gameState.Running;
        }
        break;
      case gameState.GameOver:
        break;
      case gameState.Victorious:
        break;
      default:
        break;
    }



  }

  public int toolbarInt = 0;
  public string[] toolbarStrings = new string[] {"Toolbar1", "Toolbar2", "Toolbar3"};

  void OnGUI() {
    
    switch (state) {
      case gameState.Running:
        GUI.TextField(new Rect(5, 5, 100, 40), "Lives left: " + playerHealth);
        GUI.TextField(new Rect(Screen.width - 105, 5, 100, 40), "Score: " + killScore);
        GUI.Label(new Rect(Screen.width - 105, Screen.height - 20, 200, 30), "Money: " + cash);

        if (buildMode == false) {
          foreach (Transform PlacementTile in placementGrid) {
            PlacementTile.GetComponent<Renderer>().enabled = false;
          }

          if (GUI.Button(new Rect(5, Screen.height - 45, 40, 40), "build")) {
            buildMode = true;
          }
        } else { // buildMode == true
          ray = camera.ScreenPointToRay(Input.mousePosition);
          foreach (Transform PlacementTile in placementGrid) {
            PlacementTile.GetComponent<Renderer>().enabled = true;
          }

          if (Physics.Raycast(ray, out rayHit, 50f, placementGridLayer)) {
            if (lastHitObj) {
              lastHitObj.GetComponent<Renderer>().material = originalMat; // set material of previously hit tile, back to its original material
            }

            lastHitObj = rayHit.collider.gameObject; // get the tile we currently touch with the mouse
            originalMat = lastHitObj.GetComponent<Renderer>().material;
            lastHitObj.GetComponent<Renderer>().material = hoverMat;
          } else {
            if (lastHitObj) {
              lastHitObj.GetComponent<Renderer>().material = originalMat;
              lastHitObj = null; // no longer touching a tile
            }
          }

          // if still hovering over a tile which is free, place tower!
          if (Input.GetMouseButtonDown(0) && lastHitObj) {
            if (lastHitObj.tag == "placementTileVacant") {
              TileScript lastHitScript = lastHitObj.GetComponent<TileScript>();
              lastHitScript.setTower(structuresList[0]);
              lastHitScript.setAccessible(false);
              lastHitObj.tag = "placementTileOccupied";
              placementGrid.GetComponent<GridHandler>().addTower(lastHitScript.getTileID());
              buildMode = false;
            }
          }
        }
        break;
      case gameState.Paused:
        if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 20, 200, 35), "Resume Game")) {
          //        print ("Resuming game...");
          Time.timeScale = 1;
          state = gameState.Running;
        }
        if (GUI.Button (new Rect (Screen.width/2-100,Screen.height/2+20,200,35), "Quit")) {
          //        print ("Quiting...");
        }
        break;
      case gameState.GameOver:
        break;
      case gameState.Victorious:
        break;
      default:
        break;
    }
  }

  public void setPlayerHealth(int hp) {
    playerHealth = hp;
  }

  public void setMoney(int money) {
    cash = money;
  }



  public void EnemyKilled() {
    killScore++;
  }

  public void addToPlayerHealth(int hp) {
    playerHealth += hp;
  }
}