import React from 'react';
import { connect } from 'react-redux';
import './NewNotification.scss';
import {Button} from 'reactstrap';

const NewNotification = props => (
  <div>
    <h1>New Notification updated</h1>  
    <div className="content">content</div>
    <Button color="danger">Danger!</Button>
  </div>
);

export default connect()(NewNotification);
