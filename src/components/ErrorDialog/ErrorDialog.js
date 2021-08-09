import React from 'react';
import Card from '../UI/Card';
import Button from '../UI/Button';
import classes from './ErrorDialog.module.css';

const ErrorDialog = (props) => {

  const clickHandler = (event)=>{
    event.preventDefault();
    props.onClickHandler();
  };

  return (
    <div>
      <div className={classes.backdrop}></div>
      <Card className={classes.modal}>
        <header className={classes.header}>
          <h2>{props.title}</h2>
        </header>
        <div className={classes.content}>
          <p>
            {props.message}
          </p>
        </div>
        <footer className={classes.actions}>
          <Button onClick={clickHandler}>Ok</Button>
        </footer>
      </Card>
    </div>
  );

};

export default ErrorDialog;