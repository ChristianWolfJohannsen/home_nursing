using UnityEngine;
using System.Collections;

public class MedicinFSM : FSM 
{

	public enum FSMState
	{
		None,
		NoMedicin,
		Medicin,
	}

	public FSMState curState;

	private bool gotMedicin;

	protected override void Initialize()
	{
		curState = FSMState.NoMedicin;
		gotMedicin = false;
	}

	protected override void FSMUpdate()
	{
	}

	protected void UpdateNoMedicinState()
	{

	}
}
