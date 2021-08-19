import React, { useState } from "react";
import "./NewExpense.css";
import ExpenseForm from "./ExpenseForm";

const NewExpense = (props) => {

  const [isButtonVisible, setButtonVisible] = useState(false);

  const saveExpenseDataHandler = (enteredExpenseData) => {
    const expenseData = {
      ...enteredExpenseData,
      id: Math.random().toString(),
    };

    console.log(expenseData);

    // This component exposes an event/delegate called onAddExpense.
    // This is delegate executes the function that was assigned to it (...in App.js)
    props.onAddExpense(expenseData);
    setButtonVisible(false);
  };

  const clickHandler = (event)=>{
    setButtonVisible(!isButtonVisible);
  };

  return (
    <div className="new-expense">
      {(isButtonVisible && <ExpenseForm onSaveExpenseData={saveExpenseDataHandler} OnCancelClick={clickHandler }></ExpenseForm>)}
      {!isButtonVisible && <button onClick={clickHandler}>Add New Expense</button>}
    </div>
  );
};

export default NewExpense;
