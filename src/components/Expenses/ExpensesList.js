import React from "react";
import ExpenseItem from "./ExpenseItem";
import "./ExpensesList.css";

const ExpenseList = (props) => {
  let expensesContent = (
    <h2 className="expenses-list__fallback">Found no expense</h2>
  );

  if (props.items.length > 0) {
    expensesContent = (
      <ul className="expenses-list">
        {props.items.map((item) => (
          <ExpenseItem expense={item} key={item.id}></ExpenseItem>
        ))}
      </ul>
    );
  }

  return expensesContent ;
};

export default ExpenseList;
