import React, { useState } from "react";
import "./ExpenseForm.css";

const ExpenseForm = (props) => {

  // Set state initially
  const [userInput, setUserInput] = useState({
    enteredTitle : '',
    enteredAmount: '',
    enteredDate: ''
  });

  // Handlers
  const titleChangeHandler = (event)=>{
    setUserInput((prevState) => {
      return { ...prevState, enteredTitle: event.target.value}
    })
  };

  const amountChangeHandler = (event)=>{
    setUserInput((prevState) => {
      return { ...prevState, enteredAmount: event.target.value}
    })
  };

  const dateChangeHandler = (event)=>{
    setUserInput((prevState) => {
      return { ...prevState, enteredDate: event.target.value}
    })
  };

  const submitHandler = (event)=>{
    event.preventDefault();

    const expenseData = {
      title: userInput.enteredTitle,
      amount: userInput.enteredAmount,
      date: new Date(userInput.enteredDate)
    };

    console.log(expenseData);

    // This component exposes an event/delegate, onSaveExpenseData.
    // This is delegate executes the function that was assigned to it (in NewExpense.js)
    props.onSaveExpenseData(expenseData);

    // Reset the user input values
    setUserInput(
      {
      enteredTitle : '',
      enteredAmount: '',
      enteredDate: ''
    });
  };

  const cancelHandler = () =>{
    props.OnCancelClick();
  }

  return (
    <form onSubmit={submitHandler}>
      <div className="new-expense__controls">
        <div className="new-expense__control">
            <label>Title</label>
            <input type='text' value = {userInput.enteredTitle} onChange={titleChangeHandler} />
        </div>
        <div className="new-expense__control">
            <label>Amount</label>
            <input type='number' min="0.01" step="0.01" value = {userInput.enteredAmount} onChange={amountChangeHandler} />
        </div>
        <div className="new-expense__control">
            <label>Date</label>
            <input type='date' min='2019-01-01' max='2022-12-31' value = {userInput.enteredDate} onChange={dateChangeHandler} />
        </div>
      </div>
      <div className="new-expense__actions">
          <button onClick={cancelHandler}>Cancel</button>
          <button type="submit">Add Expense</button>
      </div>
    </form>
  );
};

export default ExpenseForm;