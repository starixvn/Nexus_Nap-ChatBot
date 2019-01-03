import React, { Component } from 'react';
import { connect } from 'react-redux';
import { AvForm, AvField, AvGroup, AvInput, AvFeedback } from 'availity-reactstrap-validation';
import { Button, Label } from 'reactstrap';
import ConfirmPopup from "../../common/ConfirmPopup";
import NCBBreadcrum from '../../common/breadcrumbs';
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
        <NCBBreadcrum chatbotName="Skype Chatbot" pageTitle="New Notification"></NCBBreadcrum>
        <div className="content form">
          <AvForm>        
              <AvField 
                type="select"
                name="notificationType"
                label="Notification Type" 
                labelClass="form-label required"
                inputClass="notification-type"
                errorMessage="This field is required"
                required>

                <option>UM Licensee 1</option>
                <option>UM Licensee 2</option>
              </AvField>

              <AvField 
                name="recipient"
                label="Recipient" 
                labelClass="form-label required"
                errorMessage="This field is required"
                required>
              </AvField>

              <div className="row-has-actions">
                <AvField 
                  type="select"
                  name="notificationTemplate"
                  label="Notification Template" 
                  labelClass="form-label required"
                  inputClass="notification-template"
                  errorMessage="This field is required"
                  required>

                  <option>Weekly UM 1</option>
                  <option>Weekly UM 2</option>
                </AvField>
                <div className="action" onClick={this.contentEditConfirmPopupHandler}>
                  <div className="circle orange" >
                    <span className="icon-edit"></span>
                  </div>
                  
                  <span className="text">Edit Title & Content</span>
                </div>
                <div className="action" onClick={this.contentResetConfirmPopupHandler}>
                  <div className="circle blue">
                    <span className="icon-reset"></span>
                  </div>

                  <span className="text">Reset Title & Content</span>
                </div>
              </div>
              
              <AvField 
                name="title"
                label="Title" 
                labelClass="form-label required"
                errorMessage="This field is required"
                required>
              </AvField>
              
              <AvField 
                type="textarea"
                name="content"
                label="Content" 
                labelClass="form-label content required"
                inputClass="content"
                value={templateContent}
                required></AvField>


              <fieldset className="parameter-area">
                <legend className="bold">Parameters</legend>
                <div className="parameter-list">
                  <AvField
                    name="UMDate"
                    label="{UM date}" 
                    labelClass="form-label"
                    inputClass="m-r-15px"
                    value="UM date"></AvField>

                  <AvField
                    name="umWeekDateEN"
                    label="{UM Week Date - EN}" 
                    labelClass="form-label p-l-15px"
                    inputClass="m-r-15px"
                    value="UM Week Date - EN"></AvField>

                    <div className="w-100"></div>

                    <AvField
                      name="startTime"
                      label="{Start time}" 
                      labelClass="form-label"
                      inputClass="m-r-15px"
                      value="11:00"></AvField>

                    <AvField
                      name="endTime"
                      label="{End time}" 
                      labelClass="form-label"
                      value="End time"></AvField>

                    <div className="w-100"></div>
                    <AvField
                      name="umWeekDateCN"
                      label="{UM Week Date - CN}" 
                      labelClass="form-label"
                      value="hh:mm:ss AM/PM MM/DD/YYYY"></AvField>
                </div>            
              </fieldset>

              <div className="send-date-area">
                <AvField 
                  type="select" 
                  name="senDateTime"
                  labelClass="form-label required"
                  label="Send Date/Time" 
                  required>

                  <option>---Please select---</option>
                  <option>Send immediately</option>
                  <option>Send at</option>
                </AvField>
                <div>
                  <div className="calendar">
                    <input type="text" name="dateTime" id="dateTime" className="form-control date-time" disabled />
                  </div>
                  <span>(GMT +8)</span>
                </div>
                
              </div>
              
              <Button className="btn-submit">Submit</Button>

            </AvForm>

                 
        </div>
        <ConfirmPopup 
            showHideModalHandler={this.toggle}
            isShowPopup={isShowPopup}
            title={titlePopup}
            content={contentPopup}
            warningIcon={true}
            ></ConfirmPopup>
      </div>
    )
  }
}

export default connect()(NewNotification);
