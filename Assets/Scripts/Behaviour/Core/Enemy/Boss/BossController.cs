using UnityEngine;

using System;

using STP.Behaviour.Starter;
using STP.Core;
using STP.Manager;
using STP.Utils.BehaviourTree;
using STP.Utils.GameComponentAttributes;

using STP.Utils.BehaviourTree.Tasks;

namespace STP.Behaviour.Core.Enemy.Boss {
	public sealed class BossController : BaseCoreComponent, IDestructible {
		public static BossController Instance { get; private set; }

		[Header("Parameters")]
		public float StartHp = 100;
		[Header("Dependencies")]
		[NotNull] public BossMoveAgent             MoveAgent;
		[NotNull] public BossGunController         GunController;
		[NotNull] public BossGunRotationController GunRotationController;

		float _horizontal;
		float _vertical;

		LevelGoalManager _levelGoalManager;

		BehaviourTree _tree;

		HpSystem _hpSystem;

		public override bool HighPriorityInit => true;

		public float CurHp => _hpSystem.Hp;
		public float MaxHp => _hpSystem.MaxHp;

		public event Action<float> OnCurHpChanged;

		protected override void Awake() {
			base.Awake();
			if ( Instance ) {
				Debug.LogErrorFormat(this, "{0}.{1}: more than one {2} instance is not supported",
					nameof(BossController), nameof(Awake), nameof(BossController));
				return;
			}
			Instance = this;
		}

		void OnDestroy() {
			if ( Instance && (Instance == this) ) {
				Instance = null;
			}
		}

		protected override void InitInternal(CoreStarter starter) {
			_levelGoalManager = starter.LevelGoalManager;

			_tree = new BehaviourTree(
				new SelectorTask(
					
				)
			);

			_hpSystem = new HpSystem(StartHp);

			_hpSystem.OnHpChanged += OnHpChanged;
			_hpSystem.OnDied      += Die;

			MoveAgent.SetTarget(starter.Player.transform);
			GunRotationController.SetTarget(starter.Player.transform);
			GunRotationController.IsActive = true;

		}

		public void TakeDamage(float damage) {
			_hpSystem.TakeDamage(damage);
		}

		void OnHpChanged(float hp) {
			OnCurHpChanged?.Invoke(hp);
		}

		void Die() {
			_levelGoalManager.Advance();
			Destroy(gameObject);
		}
	}
}
