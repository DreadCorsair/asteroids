using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
	[SerializeField]
	private GameObject _shipPrefab;
	[SerializeField]
	private float _rotationSpeed;
	[SerializeField]
	private float _moveSpeed;

	private GameObject _ship;
	private Rigidbody2D _rb;
	private GameObject[] _ghosts;

	private float _screenWidth;
	private float _screenHeight;
	private Rect _screenRect;

	private const int MIN_SPEED = 1;
	private const int NUMBER_OF_GHOSTS = 8;

	void Start ()
	{
		_rotationSpeed = _rotationSpeed < MIN_SPEED ? MIN_SPEED : _rotationSpeed;
		_moveSpeed = _moveSpeed < MIN_SPEED ? MIN_SPEED : _moveSpeed;

		_ship = Instantiate(_shipPrefab, Vector2.zero, Quaternion.identity) as GameObject; 
		_rb = _ship.gameObject.GetComponent<Rigidbody2D> ();
		_ghosts = new GameObject[NUMBER_OF_GHOSTS];

		Vector3 screenBottomLeft = Camera.main.ViewportToWorldPoint (Vector2.zero);
		Vector3 screenTopRight   = Camera.main.ViewportToWorldPoint (Vector2.one);
		_screenWidth = screenTopRight.x - screenBottomLeft.x;
		_screenHeight = screenTopRight.y - screenBottomLeft.y;
		_screenRect = new Rect (screenBottomLeft, new Vector2(_screenWidth, _screenHeight));

		CreateGhosts ();
	}
	
	void Update () 
	{
		if (_rb == null)
			return;

		if (Input.GetKey (KeyCode.W)) 
			_rb.AddForce(_ship.transform.up * _moveSpeed);

		if (Input.GetKey (KeyCode.A)) 
			_ship.transform.Rotate(Vector3.forward * _rotationSpeed);

		if (Input.GetKey (KeyCode.S))
			_rb.AddForce (-_ship.transform.up * _moveSpeed);

		if (Input.GetKey (KeyCode.D)) 
			_ship.transform.Rotate(Vector3.back * _rotationSpeed);

		if (Input.GetKeyDown (KeyCode.Backspace))
			Log ();

		PositionGhosts ();
		SwapGhost ();
	}

	private void CreateGhosts()
	{
		if (_ghosts == null)
			return;

		for(int i = 0; i < NUMBER_OF_GHOSTS; i++)
		{
			_ghosts[i] = Instantiate(_shipPrefab, Vector2.zero, Quaternion.identity) as GameObject;
		}
	}

	private void PositionGhosts()
	{
		Vector2 ghostPosition = _ship.transform.position;
		
		ghostPosition.x = _ship.transform.position.x + _screenWidth;
		ghostPosition.y = _ship.transform.position.y;
		_ghosts[0].transform.position = ghostPosition;
		
		// Bottom-right
		ghostPosition.x = _ship.transform.position.x + _screenWidth;
		ghostPosition.y = _ship.transform.position.y - _screenHeight;
		_ghosts[1].transform.position = ghostPosition;
		
		// Bottom
		ghostPosition.x = _ship.transform.position.x;
		ghostPosition.y = _ship.transform.position.y - _screenHeight;
		_ghosts[2].transform.position = ghostPosition;
		
		// Bottom-left
		ghostPosition.x = _ship.transform.position.x - _screenWidth;
		ghostPosition.y = _ship.transform.position.y - _screenHeight;
		_ghosts[3].transform.position = ghostPosition;
		
		// Left
		ghostPosition.x = _ship.transform.position.x - _screenWidth;
		ghostPosition.y = _ship.transform.position.y;
		_ghosts[4].transform.position = ghostPosition;
		
		// Top-left
		ghostPosition.x = _ship.transform.position.x - _screenWidth;
		ghostPosition.y = _ship.transform.position.y + _screenHeight;
		_ghosts[5].transform.position = ghostPosition;
		
		// Top
		ghostPosition.x = _ship.transform.position.x;
		ghostPosition.y = _ship.transform.position.y + _screenHeight;
		_ghosts[6].transform.position = ghostPosition;
		
		// Top-right
		ghostPosition.x = _ship.transform.position.x + _screenWidth;
		ghostPosition.y = _ship.transform.position.y + _screenHeight;
		_ghosts[7].transform.position = ghostPosition;
		
		for(int i = 0; i < NUMBER_OF_GHOSTS; i++)
		{
			_ghosts[i].transform.rotation = _ship.transform.rotation;
		}
	}

	private void SwapGhost()
	{
		if (ShipIsVisible (_ship))
			return;

		foreach (var ghost in _ghosts) 
		{
			if(ShipIsVisible(ghost))
			{
				_ship.transform.position = ghost.transform.position;
				break;
			}
		}
	}

	private bool ShipIsVisible(GameObject ship)
	{
		if (ship == null)
			return false;

		var shipPosition = ship.transform.position;
		Vector2 shipScale = ship.transform.localScale;
		Vector2[] shipVertices = new Vector2[4];
		shipVertices[0] = new Vector2(shipPosition.x - shipScale.x / 2f, shipPosition.y - shipScale.y / 2f);
		shipVertices[1] = new Vector2(shipPosition.x + shipScale.x / 2f, shipPosition.y + shipScale.y / 2f);
		shipVertices[2] = new Vector2(shipPosition.x + shipScale.x / 2f, shipPosition.y - shipScale.y / 2f);
		shipVertices[3] = new Vector2(shipPosition.x - shipScale.x / 2f, shipPosition.y + shipScale.y / 2f);

		foreach(var vertex in shipVertices)
		{
			if(!_screenRect.Contains(vertex))
			   continue;
			return true;
		}
		
		return false;
	}
	
	void OnDrawGizmos()
	{
		//draw camera
		Gizmos.color = Color.white;
		
		Vector2 screenBottomLeft  = Camera.main.ViewportToWorldPoint (Vector2.zero);
		Vector2 screenTopRight    = Camera.main.ViewportToWorldPoint (Vector2.one);
		Vector2 screenBottomRight = new Vector2 (screenTopRight.x,   screenBottomLeft.y);
		Vector2 screenTopLeft     = new Vector2 (screenBottomLeft.x, screenTopRight.y);

		Gizmos.DrawLine (screenBottomLeft,  screenTopLeft);
		Gizmos.DrawLine (screenTopLeft,     screenTopRight);
		Gizmos.DrawLine (screenTopRight,    screenBottomRight);
		Gizmos.DrawLine (screenBottomRight, screenBottomLeft);

		//HACK
		if (!Application.isPlaying)
			return;

		//draw ghosts
		Gizmos.color = Color.red;

		Vector2 ghostScale = _ship.transform.localScale;
		for (int i = 0; i < NUMBER_OF_GHOSTS; i++) 
		{
			Vector3 ghostPostion = _ghosts[i].transform.position;

			Vector2 ghostBottomLeft  = new Vector2(ghostPostion.x - ghostScale.x / 2f, ghostPostion.y - ghostScale.y / 2f);
			Vector2 ghostTopRight    = new Vector2(ghostPostion.x + ghostScale.x / 2f, ghostPostion.y + ghostScale.y / 2f);
			Vector2 ghostBottomRigth = new Vector2(ghostPostion.x + ghostScale.x / 2f, ghostPostion.y - ghostScale.y / 2f);
			Vector2 ghostTopLeft     = new Vector2(ghostPostion.x - ghostScale.x / 2f, ghostPostion.y + ghostScale.y / 2f);

			Gizmos.DrawLine(ghostBottomLeft, ghostTopRight);
			Gizmos.DrawLine(ghostBottomRigth, ghostTopLeft);
		}

		//draw rect
		Gizmos.color = Color.green;
		Gizmos.DrawLine (new Vector2(_screenRect.xMin, _screenRect.yMin), new Vector2(_screenRect.xMin, _screenRect.yMax));
		Gizmos.DrawLine (new Vector2(_screenRect.xMin, _screenRect.yMax), new Vector2(_screenRect.xMax, _screenRect.yMax));
		Gizmos.DrawLine (new Vector2(_screenRect.xMax, _screenRect.yMax), new Vector2(_screenRect.xMax, _screenRect.yMin));
		Gizmos.DrawLine (new Vector2(_screenRect.xMax, _screenRect.yMin), new Vector2(_screenRect.xMin, _screenRect.yMin));

		//draw out ship
		if (ShipIsVisible(_ship))
			return;

		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere (_ship.transform.position, _ship.transform.localScale.y);
	}

	void Log()
	{
		Debug.Log("Rect position: " + _screenRect.position);
	}
}