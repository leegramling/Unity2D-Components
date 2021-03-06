using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Matcha.Unity
{
	public class M : BaseBehaviour
	{
		// ignore layer collisions *by name* instead of by layer ID
		public static void IgnoreLayerCollisions(string layer1, string layer2, bool status)
		{
			Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(layer1), LayerMask.NameToLayer(layer2), status);
		}

		// same as above but ignores layer collisions between *this gameObject* and other layer
		public static void IgnoreLayerCollisionsWith(GameObject gameObject, string layer2, bool status)
		{
			Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer(layer2), status);
		}

		// returns an int constant describing which side the GameObject has been hit on
		public static int HorizontalSideHit(GameObject currentGo, Collider2D coll)
		{
			Vector3 relativePosition = currentGo.transform.InverseTransformPoint(coll.transform.position);

			// if scale is positive: ie, facing right
			if (currentGo.transform.lossyScale.x >= 1)
			{
				if (relativePosition.x > 0)
				{
					return RIGHT;
				}
				
				return LEFT;
			}

			if (currentGo.transform.lossyScale.x <= -1)
			{
				if (relativePosition.x < 0)
				{
					return RIGHT;
				}

				return LEFT;
			}
			
			return ERROR;
		}

		// same as above, but returns the vertical side that was hit
		public static int VerticalSideHit(GameObject currentGo, Collider2D coll)
		{
			Vector3 relativePosition = currentGo.transform.InverseTransformPoint(coll.transform.position);

			// if scale is positive: ie, upright
			if (currentGo.transform.lossyScale.y >= 1)
			{
				if (relativePosition.y > 0)
				{
					return TOP;
				}
				
				return BOTTOM;
			}

			if (currentGo.transform.lossyScale.y <= -1)
			{
				if (relativePosition.y < 0)
				{
					return TOP;
				}

				return BOTTOM;
			}
			
			return ERROR;
		}

		// HexToColor was written by Danny Lawrence, and appears here unmodified.
		// It is reproduced under a Creative Common license — http://creativecommons.org/licenses/by-sa/3.0/
		public static Color HexToColor(string hex)
		{
			byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
			byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
			byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

			return new Color32(r, g, b, 255);
		}

		// returns Vector2 coordinates for lobbing a projectile
		public static Vector2 LobProjectile(Weapon weapon, Transform origin, Transform target)
		{
			float distance;
			float yDifference;
			float angleToPoint;
			float distanceFactor;
			float distanceCompensation;
			float speedCompensation;
			float angleCorrection;
			float speed;

			// the below formula is accurate given the following settings: gravity of .5,
			// angular drag of .05, mass of 1, and projectile speeds between 8 and 20

			distance = target.position.x - origin.position.x;
			yDifference = target.position.y - origin.position.y;
			angleToPoint = (float)Math.Atan2(target.position.y - origin.position.y, target.position.x - origin.position.x);
			speed = weapon.speed;

			// compensate for various projectile speeds
			// in terms of accuracy, the data set below supports speeds between 8 and 20
			if (speed < 8) {
				speedCompensation = 3.2f;
			}
			else if (speed >= 8 && speed < 9)
			{
				speedCompensation = 2.2f;
			}
			else if (speed >= 9 && speed < 10)
			{
				speedCompensation = 1.85f;
			}
			else if (speed >= 10 && speed < 11)
			{
				speedCompensation = 1.50f;
			}
			else if (speed >= 11 && speed < 12)
			{
				speedCompensation = 1.25f;
			}
			else if (speed >= 12 && speed < 13)
			{
				speedCompensation = 1f;
			}
			else if (speed >= 13 && speed < 14)
			{
				speedCompensation = .85f;
			}
			else if (speed >= 14 && speed < 15)
			{
				speedCompensation = .75f;
			}
			else if (speed >= 15 && speed < 16)
			{
				speedCompensation = .65f;
			}
			else if (speed >= 16 && speed < 17)
			{
				speedCompensation = .55f;
			}
			else if (speed >= 17 && speed < 18)
			{
				speedCompensation = .5f;
			}
			else if (speed >= 18 && speed < 19)
			{
				speedCompensation = .45f;
			}
			else if (speed >= 19 && speed < 20)
			{
				speedCompensation = .4f;
			}
			else
			{
				speedCompensation = .35f;
			}

			// compensate for both positive and negative distances between origin and target
			distanceFactor = .034f * speedCompensation;

			if (yDifference >= -2f) {
				distanceCompensation = .001f * speedCompensation;
			}
			else
			{
				distanceCompensation = .00065f * speedCompensation;
			}

			distanceFactor += yDifference * distanceCompensation;
			angleCorrection = (float)(3.14 * 0.18) * (distance * distanceFactor);

			return new Vector2((float)Math.Cos(angleToPoint + angleCorrection) * speed,
							 (float)Math.Sin(angleToPoint + angleCorrection) * speed);
		}

		public static void PositionInHUD(RectTransform rectTrans, Text text, int anchor, float x, float y)
		{
			switch (anchor)
			{
				case TOP_LEFT:
					y = -y;
					rectTrans.anchorMin = new Vector2(0, 1);
					rectTrans.anchorMax = new Vector2(0, 1);
					rectTrans.anchoredPosition = new Vector3(x, y + 38f, 0f);
					text.alignment = TextAnchor.UpperLeft;
					break;
				case TOP_CENTER:
					y = -y;
					rectTrans.anchorMin = new Vector2(0.5f, 1);
					rectTrans.anchorMax = new Vector2(0.5f, 1);
					rectTrans.anchoredPosition = new Vector3(x - (rectTrans.sizeDelta.x / 2), y + 38f, 0f);
					text.alignment = TextAnchor.UpperCenter;
					break;
				case TOP_RIGHT:
					y = -y;
					rectTrans.anchorMin = new Vector2(1, 1);
					rectTrans.anchorMax = new Vector2(1, 1);
					rectTrans.anchoredPosition = new Vector3(-(rectTrans.sizeDelta.x + x - 6f), y + 38f, 0f);
					text.alignment = TextAnchor.UpperRight;
					break;
				case MIDDLE_LEFT:
					rectTrans.anchorMin = new Vector2(0, 0.5f);
					rectTrans.anchorMax = new Vector2(0, 0.5f);
					rectTrans.anchoredPosition = new Vector3(x, y + 62f, 0f);
					text.alignment = TextAnchor.MiddleLeft;
					break;
				case MIDDLE_CENTER:
					rectTrans.anchorMin = new Vector2(0.5f, 0.5f);
					rectTrans.anchorMax = new Vector2(0.5f, 0.5f);
					rectTrans.anchoredPosition = new Vector3(x - (rectTrans.sizeDelta.x / 2), y + 62f, 0f);
					text.alignment = TextAnchor.MiddleCenter;
					break;
				case MIDDLE_RIGHT:
					rectTrans.anchorMin = new Vector2(1, 0.5f);
					rectTrans.anchorMax = new Vector2(1, 0.5f);
					rectTrans.anchoredPosition = new Vector3(-(rectTrans.sizeDelta.x + x - 6f), y + 62f, 0f);
					text.alignment = TextAnchor.MiddleRight;
					break;
				case BOTTOM_LEFT:
					rectTrans.anchorMin = new Vector2(0, 0);
					rectTrans.anchorMax = new Vector2(0, 0);
					rectTrans.anchoredPosition = new Vector3(x, y + 87f, 0f);
					text.alignment = TextAnchor.LowerLeft;
					break;
				case BOTTOM_CENTER:
					rectTrans.anchorMin = new Vector2(0.5f, 0);
					rectTrans.anchorMax = new Vector2(0.5f, 0);
					rectTrans.anchoredPosition = new Vector3(x - (rectTrans.sizeDelta.x / 2), y + 87f, 0f);
					text.alignment = TextAnchor.LowerCenter;
					break;
				case BOTTOM_RIGHT:
					rectTrans.anchorMin = new Vector2(1, 0);
					rectTrans.anchorMax = new Vector2(1, 0);
					rectTrans.anchoredPosition = new Vector3(-(rectTrans.sizeDelta.x + x - 6f), y + 87f, 0f);
					text.alignment = TextAnchor.LowerRight;
					break;
				default:
					Assert.IsTrue(false, "** Default Case Reached **");
					break;
			}
		}

		public static void PositionInHUD(RectTransform rectTrans, SpriteRenderer spriteRenderer, int anchor, float x, float y)
		{
			// Beware: here be platform-specific magic numbers! 
			// they are offsets to help zero-out all coordinates.

		#if UNITY_STANDALONE_OS
			float platformSpecificFudgeFactor = 58f;
		#endif

		#if UNITY_IOS
			float platformSpecificFudgeFactor = 225f;
		#endif

			float xOffset = spriteRenderer.bounds.size.x / 2 * platformSpecificFudgeFactor;
			float yOffset = spriteRenderer.bounds.size.y * platformSpecificFudgeFactor;
			switch (anchor)
			{
				case TOP_LEFT:
					y = -y;
					rectTrans.anchorMin = new Vector2(0, 1);
					rectTrans.anchorMax = new Vector2(0, 1);
					rectTrans.anchoredPosition = new Vector3(x + xOffset, y - yOffset, 0f);
					break;
				case TOP_CENTER:
					y = -y;
					rectTrans.anchorMin = new Vector2(0.5f, 1);
					rectTrans.anchorMax = new Vector2(0.5f, 1);
					rectTrans.anchoredPosition = new Vector3(x - (rectTrans.sizeDelta.x / 2), y - yOffset, 0f);
					break;
				case TOP_RIGHT:
					y = -y;
					rectTrans.anchorMin = new Vector2(1, 1);
					rectTrans.anchorMax = new Vector2(1, 1);
					rectTrans.anchoredPosition = new Vector3(-(rectTrans.sizeDelta.x + x) - xOffset, y - yOffset, 0f);
					break;
				case MIDDLE_LEFT:
					rectTrans.anchorMin = new Vector2(0, 0.5f);
					rectTrans.anchorMax = new Vector2(0, 0.5f);
					rectTrans.anchoredPosition = new Vector3(x + xOffset, y - (yOffset / 2), 0f);
					break;
				case MIDDLE_CENTER:
					rectTrans.anchorMin = new Vector2(0.5f, 0.5f);
					rectTrans.anchorMax = new Vector2(0.5f, 0.5f);
					rectTrans.anchoredPosition = new Vector3(x - (rectTrans.sizeDelta.x / 2), y - (yOffset / 2), 0f);
					break;
				case MIDDLE_RIGHT:
					rectTrans.anchorMin = new Vector2(1, 0.5f);
					rectTrans.anchorMax = new Vector2(1, 0.5f);
					rectTrans.anchoredPosition = new Vector3(-(rectTrans.sizeDelta.x + x) - xOffset, y - (yOffset / 2), 0f);
					break;
				case BOTTOM_LEFT:
					rectTrans.anchorMin = new Vector2(0, 0);
					rectTrans.anchorMax = new Vector2(0, 0);
					rectTrans.anchoredPosition = new Vector3(x + xOffset, y, 0f);
					break;
				case BOTTOM_CENTER:
					rectTrans.anchorMin = new Vector2(0.5f, 0);
					rectTrans.anchorMax = new Vector2(0.5f, 0);
					rectTrans.anchoredPosition = new Vector3(x - (rectTrans.sizeDelta.x / 2), y, 0f);
					break;
				case BOTTOM_RIGHT:
					rectTrans.anchorMin = new Vector2(1, 0);
					rectTrans.anchorMax = new Vector2(1, 0);
					rectTrans.anchoredPosition = new Vector3(-(rectTrans.sizeDelta.x + x) - xOffset, y, 0f);
					break;
				default:
					Assert.IsTrue(false, "** Default Case Reached **");
					break;
			}
		}
	}
}
