using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    [TestFixture]
    public class NewTestScript
    {
        GameObject m_Tank, m_Shell, m_Camera, m_LevelObject, m_GameManagerObject, m_CameraObject;
        const float k_InputValue = 15f;
        const float k_LaunchForce = 31f;
        const int k_Roundnumber = 0;
        Vector3 k_MockPosition = new Vector3(-3, 0, 34.54757f);

        TankHealth m_TankHealth;
        TankShooting m_TankShooting;
        TankMovement m_TankMovement;
        ShellExplosion m_ShellExplosion;
        GameManager m_GameManager;

        [SetUp]
        public void SetUp()
        {
            m_Tank = GameObject.Instantiate((GameObject)Resources.Load("Tank"));
            m_Tank.SetActive(false);
        }

        [Test]
        public void TankObjectShouldNotBeNull()
        {
            m_Tank.SetActive(true);
            Assert.NotNull(m_Tank, "Tank game object should not be null");
        }

        [UnityTest]
        public IEnumerator TankObjectShouldGetInActive_OnCollisionWithTwoShellObjects()
        {
            m_Tank.SetActive(true);

            TankObjectAndShellObjectCollision();
            yield return null;

            TankObjectAndShellObjectCollision();

            yield return null;
            Assert.IsFalse(m_Tank.activeSelf, "Tank object should be inactive in heirarchy on collision with two shell objects");
        }

        [UnityTest]
        public IEnumerator TankObjectHealthShouldGetDecreased_OnCollisionWithShellObject()
        {
            m_Tank.SetActive(true);

            m_TankHealth = m_Tank.GetComponent<TankHealth>();
            var tankHealthBeforeCollisionWithShell = m_TankHealth.CurrentHealth;

            TankObjectAndShellObjectCollision();
            yield return null;

            var tankHealthAfterCollisionWithShell = m_TankHealth.CurrentHealth;
            Assert.Less(tankHealthAfterCollisionWithShell, tankHealthBeforeCollisionWithShell, "Health of Tank should get reduced after collision with shell");
        }

        [UnityTest]
        public IEnumerator TankObjectShouldMove_DueToExplosionForce_OfShellObject()
        {
            m_Tank.SetActive(true);

            m_LevelObject = GameObject.Instantiate((GameObject)Resources.Load("LevelArt"));
            var tankPositionBeforeCollisionWithShell = m_Tank.transform.position;

            m_Shell = GameObject.Instantiate((GameObject)Resources.Load("Shell"), new Vector3(m_Tank.transform.position.x, m_Tank.transform.position.y, m_Tank.transform.position.z + 3), Quaternion.identity);
            yield return null;

            var tankPositionAfterCollisionWithShell = m_Tank.transform.position;

            Assert.That(tankPositionAfterCollisionWithShell, Is.Not.EqualTo(tankPositionBeforeCollisionWithShell), "Tank position should change because of explosion force of the shell.");

        }

        [UnityTest]
        public IEnumerator ShellObjectShouldBeCreated_OnFireMethod_InUpdateMethod()
        {
            m_Tank.SetActive(true);

            m_TankShooting = m_Tank.GetComponent<TankShooting>();

            m_TankShooting.CurrentLaunchForce = k_LaunchForce;
            m_TankShooting.IsFired = false;

            yield return null;

            m_Shell = GameObject.Find("Shell(Clone)");
            Assert.That(m_Shell, Is.Not.EqualTo(null), "Tank Object should be created and should not be null");
        }

        [UnityTest]
        public IEnumerator TankObjectShouldMove_OnMovementInputValue()
        {
            m_Tank.SetActive(true);

            m_TankMovement = m_Tank.GetComponent<TankMovement>();
            var tankPositionBeforeMovementInputValue = m_Tank.transform.position;

            m_TankMovement.MovementInputValue = k_InputValue;

            yield return new WaitForFixedUpdate();

            var tankPositionAfterMovementInputValue = m_Tank.transform.position;
            var movement = m_Tank.transform.forward * m_TankMovement.MovementInputValue * m_TankMovement.m_Speed * Time.deltaTime;

            Assert.That(tankPositionAfterMovementInputValue, Is.EqualTo(tankPositionBeforeMovementInputValue + movement), "Tank Object position should get change after providing Movement input value.");
        }

        [UnityTest]
        public IEnumerator TankObjectShouldRotate_OnTurnInputValue()
        {
            m_Tank.SetActive(true);

            m_TankMovement = m_Tank.GetComponent<TankMovement>();
            var tankRotationBeforeRotationInputValue = m_Tank.transform.rotation;

            m_TankMovement.TurnInputValue = k_InputValue;

            yield return new WaitForFixedUpdate();

            var tankRotationAfterRotationInputValue = m_Tank.transform.rotation;
            var turn = m_TankMovement.TurnInputValue * m_TankMovement.m_TurnSpeed * Time.deltaTime;
            Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);

            Assert.That(tankRotationAfterRotationInputValue, Is.EqualTo(tankRotationBeforeRotationInputValue * turnRotation), "Tank Object rotation should get change after providing Turn input value.");
        }

        [UnityTest]
        public IEnumerator CameraGameObject_ShouldChangePosition_WhenTankObjectChangesItsPosition()
        {
            LoadGameManagerAndCameraRig();

            var cameraPosition = m_GameManager.m_CameraControl.transform.position;
            yield return null;

            var tank = GameObject.Find("Tank(Clone)");
            yield return null;

            tank.transform.position = k_MockPosition;
            yield return new WaitForFixedUpdate();

            Assert.That(cameraPosition, Is.Not.EqualTo(m_GameManager.m_CameraControl.transform.position), "Camera must changes position when tank changes its position");
        }

        [UnityTest]
        public IEnumerator RoundNumber_ShouldGetDisplayed_OnGameStarts()
        {
            LoadGameManagerAndCameraRig();

            yield return null;
            Assert.Greater(m_GameManager.RoundNumber, k_Roundnumber, "Test Should display Round number after game starts ");
        }

        void LoadGameManagerAndCameraRig()
        {
            m_GameManagerObject = GameObject.Instantiate((GameObject)Resources.Load("GameManager"));
            m_CameraObject = GameObject.Instantiate((GameObject)Resources.Load("CameraRig"));

            var cameraControl = m_CameraObject.GetComponent<CameraControl>();
            m_GameManager = m_GameManagerObject.GetComponent<GameManager>();

            m_GameManager.m_CameraControl = cameraControl;
        }

        void TankObjectAndShellObjectCollision()
        {
            m_Shell = GameObject.Instantiate((GameObject)Resources.Load("Shell"));
            m_Shell.transform.position = m_Tank.transform.position;
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.Destroy(m_Tank);
            GameObject.Destroy(m_Shell);
            GameObject.Destroy(m_LevelObject);
        }
    }
}

