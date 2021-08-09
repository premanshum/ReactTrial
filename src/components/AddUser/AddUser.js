import "./AddUser.css";
import React, { useState } from "react";
import Card from "../UI/Card";
import Button from "../UI/Button";
import ErrorDialog from '../ErrorDialog/ErrorDialog';

const AddUser = (props) => {

  // Set state initially
  const [userInput, setUserInput] = useState(
    {
      userName: '',
      userAge: ''
    });

  const [errorState, setErrorState] = useState();

  // Handlers
  const nameChangeHandler = (event) => {
    setUserInput((prevState) => {
      return { ...prevState, userName: event.target.value }
    })
  };

  const ageChangeHandler = (event) => {
    setUserInput((prevState) => {
      return { ...prevState, userAge: event.target.value }
    })
  };

  const submitHandler = (event) => {
    event.preventDefault();
    if (userInput.userName.trim().length === 0 || userInput.userAge.trim().length === 0) {
      setErrorState(
        {
          title: 'An error occured',
          message: 'Please enter the values!'
        });
      console.log('Invalid input');
      return;
    }

    if (userInput.userAge < 1) {
      setErrorState(
        {
          title: 'An error occured',
          message: 'Age cannot be less than 1'
        });
      console.log('Age cannot be less than 1');
      return;
    }

    console.log(userInput);
    props.onAddUser(userInput);
    setUserInput(
      {
        userName: '',
        userAge: ''
      });
  };

  const errorOkHandler = () => {
    setErrorState(null);
  };

  return (
    <div>
      {errorState &&
        <ErrorDialog title={errorState.title}
          message={errorState.message}
          onClickHandler={errorOkHandler}></ErrorDialog>}

      <Card>
        <form onSubmit={submitHandler}>
          <div className="">
            <div className="">
              <label>Name</label>
              <input type='text' value={userInput.userName} onChange={nameChangeHandler} />
            </div>
            <div className="">
              <label>Age</label>
              <input type='number' value={userInput.userAge} onChange={ageChangeHandler} />
            </div>
          </div>
          <div className="">
            <Button type="submit">Add</Button>
          </div>
        </form>
      </Card>
    </div>
  );
};

export default AddUser;