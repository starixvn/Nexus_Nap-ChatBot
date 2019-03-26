import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Form, FormGroup, Label, Input, FormFeedback } from 'reactstrap';
import { Button } from 'reactstrap';
import ConfirmPopup from "../../common/ConfirmPopup";
import {NCBBreadcrum} from '../../common/breadcrumbs';
import "../../../fonts/style.css";
import './NewNotification.scss';

class NewNotification extends Component {
  constructor(props) {
    super(props);
    this.toggle = this.toggle.bind(this);
    this.contentEditConfirmPopupHandler = this.contentEditConfirmPopupHandler.bind(this);
    this.contentResetConfirmPopupHandler = this.contentResetConfirmPopupHandler.bind(this);
  }

  contentEditConfirmPopupHandler () {
    this.props.contentEditConfirmPopupHandler();
  }

  contentResetConfirmPopupHandler () {
    this.props.contentResetConfirmPopupHandler();
  }

  toggle() {
    this.props.toggle();
  }

  render() {
    const templateContent = " Thank you for your kind attention!";
    const {       
        isShowPopup,
        titlePopup,        
        contentPopup } = this.props;
    return (
      <div>
        <NCBBreadcrum chatbotName="Skype Chatbot" pageTitle="New Notification" />
        <div className="content form">
          <Form>        
              <FormGroup>
                    <Label for="notificationType" className="form-label required">Notification Type</Label>
                    <Input type="select" name="select" id="notificationType" className="notification-type" invalid>
                        <option>---Please select---</option>
                        <option>UM Licensee 1</option>
                        <option>UM Licensee 2</option>
                    </Input>
                    <FormFeedback>This field is required</FormFeedback>
              </FormGroup>

              <FormGroup>
                  <Label for="recipient" className="form-label required">Recipient</Label>
                  <Input invalid id="recipient" />
                  <FormFeedback>This field is required</FormFeedback>
              </FormGroup>

              <div className="row-has-actions">
                <FormGroup>
                      <Label for="notificationTemplate" className="form-label required">Notification Template</Label>
                      <Input type="select" name="select" id="notificationTemplate" invalid className="notification-template">
                          <option>---Please select---</option>
                          <option>Weekly UM 1</option>
                          <option>Weekly UM 2</option>
                      </Input>
                      <FormFeedback>This field is required</FormFeedback>
                </FormGroup>
                <div className="action" onClick={this.contentEditConfirmPopupHandler}>
                  <div className="circle orange" >
                    <span className="icon-edit" />
                  </div>
                  
                  <span className="text">Edit Title & Content</span>
                </div>
                <div className="action" onClick={this.contentResetConfirmPopupHandler}>
                  <div className="circle blue">
                    <span className="icon-reset" />
                  </div>

                  <span className="text">Reset Title & Content</span>
                </div>
              </div>
              
              <FormGroup>
                  <Label for="title" className="form-label required">Title</Label>
                  <Input invalid id="title" />
                  <FormFeedback>This field is required</FormFeedback>
              </FormGroup>

              <FormGroup>
                  <Label for="content" className="form-label content required">Content</Label>
                  <Input type="textarea" invalid id="content" className="content" value={templateContent} />
                  <FormFeedback>This field is required</FormFeedback>
              </FormGroup>

              <fieldset className="parameter-area">
                <legend className="bold">Parameters</legend>
                <div className="parameter-list">
                  <FormGroup>
                    <Label for="UMDate" className="form-label">UM date</Label>
                    <Input id="UMDate" className="m-r-15px" />
                    <FormFeedback>This field is required</FormFeedback>
                 </FormGroup>

                 <FormGroup>
                    <Label for="umWeekDateEN" className="form-label p-l-15px">UM Week Date - EN</Label>
                    <Input id="umWeekDateEN" className="m-r-15px" />
                    <FormFeedback>This field is required</FormFeedback>
                 </FormGroup>

                  <div className="w-100" />

                  <FormGroup>
                    <Label for="startTime" className="form-label p-l-15px">Start time</Label>
                    <Input id="startTime" className="m-r-15px" value="11:00" />
                    <FormFeedback>This field is required</FormFeedback>
                 </FormGroup>

                 <FormGroup>
                    <Label for="endTime" className="form-label p-l-15px">End time</Label>
                    <Input id="endTime" className="m-r-15px" value="End time" />
                    <FormFeedback>This field is required</FormFeedback>
                 </FormGroup>

                 <div className="w-100" />

                 <FormGroup>
                    <Label for="umWeekDateCN" className="form-label p-l-15px">UM Week Date - CN</Label>
                    <Input id="umWeekDateCN" className="m-r-15px" value="hh:mm:ss AM/PM MM/DD/YYYY" />
                    <FormFeedback>This field is required</FormFeedback>
                 </FormGroup>
                </div>            
              </fieldset>

              <div className="send-date-area">
                <FormGroup>
                  <Label for="senDateTime" className="form-label required">Send Date/Time</Label>
                  <Input type="select" name="select" id="senDateTime" invalid>
                      <option>---Please select---</option>
                      <option>Send immediately</option>
                      <option>Send at</option>
                  </Input>
                  <FormFeedback>This field is required</FormFeedback>
                </FormGroup>
                <div>
                  <div className="calendar">
                    <input type="text" name="dateTime" id="dateTime" className="form-control date-time" disabled />
                  </div>
                  <span>(GMT +8)</span>
                </div>
                
              </div>
              
              <Button className="btn-submit">Submit</Button>

            </Form>

                 
        </div>
        {
          isShowPopup &&
          <ConfirmPopup 
              showHideModalHandler={this.toggle}            
              title={titlePopup}
              content={contentPopup}
              warningIcon
          />
        }
      </div>
    );
  }
}

export default connect()(NewNotification);
