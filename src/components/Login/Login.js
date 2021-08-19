import React, { useState, useEffect, useReducer } from 'react';
import classes from './Login.module.css';
import Card from '../UI/Card';
import Button from '../UI/Button';

const formStateReducer = (state, action)=>{

};

const initialFormState = {
  emailValue : '',
  passwordValue : '',
  isEmailValid: false,
  isPasswordValid: false
};

const Login = (props) => {

  // Reducer
  const [formState, setFormState] = useReducer(formStateReducer, initialFormState);

  // State
  const [emailIsValid, setEmailIsValid] = useState();
  const [passwordIsValid, setpasswordIsValid] = useState();
  const [enteredEmail, setEnteredEmail] = useState('');
  const [enteredPassword, setEnteredPassword] = useState('');
  const [formIsValid, setFormIsValid] = useState();

  useEffect(()=>{
    if(emailIsValid && passwordIsValid){
      setFormIsValid(true)
    }
  }, [emailIsValid, passwordIsValid])

  useEffect(()=>{
    if(enteredEmail.length>0 && enteredEmail.includes('@')){
      setEmailIsValid(true);
    }
  }, [enteredEmail]);

  useEffect(()=>{
    if(enteredPassword.length >=7){
      setpasswordIsValid(true);
    }
  }, [enteredPassword]);

  // Handlers
  const emailChangeHandler = (event)=>{
    setEnteredEmail(event.target.value);
  };

  const validateEmailHandler = (event)=>{
    setEmailIsValid(enteredEmail.length !== 0 && enteredEmail.includes('@'));
  };

  const passwordChangeHandler = (event)=>{
    setEnteredPassword(event.target.value);
  };

  const validatePasswordHandler = (event)=>{
    setpasswordIsValid(enteredPassword.length >= 7);
  };

  const submitHandler = (event)=>{
    event.preventDefault();
    if(formIsValid){
      props.onLogin();
    }
    else{

    }
  };
  // Handlers Ends

  return (
    <Card className={classes.login}>
      <form onSubmit={submitHandler}>
      <div
          className={`${classes.control} ${
            emailIsValid === false ? classes.invalid : ''
          }`}
        >
          <label htmlFor="email">E-Mail</label>
          <input
            type="email"
            id="email"
            value={enteredEmail}
            onChange={emailChangeHandler}
            onBlur={validateEmailHandler}
          />
        </div>
        <div
          className={`${classes.control} ${
            passwordIsValid === false ? classes.invalid : ''
          }`}
        >
          <label htmlFor="password">Password</label>
          <input
            type="password"
            id="password"
            value={enteredPassword}
            onChange={passwordChangeHandler}
            onBlur={validatePasswordHandler}
          />
        </div>
        <div className={classes.actions}>
          <Button type="submit" className={classes.btn} disabled={!formIsValid}>
            Login
          </Button>
        </div>
      </form>
    </Card>);
}

export default Login;