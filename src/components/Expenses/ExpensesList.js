import React from "react";
import ExpenseItem from "./ExpenseItem";
import "./ExpensesList.css";

const ExpenseList = (props) => {
  let expensesContent = (
    <h2 className="expenses-list__fallback">Found no expense</h2>
  );

  if (props.items.length > 0) {
    expensesContent = (
      <div className="expenses ">
        <ul className="expenses-list">
          {props.items.map((item) => (
            <ExpenseItem expense={item} key={item.id}></ExpenseItem>
          ))}
        </ul>
      </div>
    );
  }

  return expensesContent;
};

export default ExpenseList;
