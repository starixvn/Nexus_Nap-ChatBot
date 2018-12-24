import React from 'react';
import { connect } from 'react-redux';
import { AvForm, AvField, AvGroup, AvInput, AvFeedback } from 'availity-reactstrap-validation';
import { Button, Label, FormGroup } from 'reactstrap';
import "../../../fonts/style.css";
import NCBBreadcrum from '../../common/breadcrumbs';

import './NewNotification.scss';

const NewNotification = props => {
  const templateContent = " Thank you for your kind attention!";

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
            required>

            <option>UM Licensee 1</option>
            <option>UM Licensee 2</option>
          </AvField>

          <AvGroup>
            <Label for="recipient" className="form-label required">Recipient</Label>
            <AvInput name="recipient" id="recipient" required />
          </AvGroup>
          <div className="row-has-actions">
            <AvField 
              type="select"
              name="notificationTemplate"
              label="Notification Template" 
              labelClass="form-label required"
              inputClass="notification-template"
              required>

              <option>Weekly UM 1</option>
              <option>Weekly UM 2</option>
            </AvField>
            <div className="action">
              <div className="circle orange">
                <span className="icon-edit"></span>
              </div>
              
              Edit Title & Content
            </div>
            <div className="action">
              <div className="circle blue">
                <span className="icon-reset"></span>
              </div>

              Reset Title & Content
            </div>
          </div>
          

          <AvGroup>
            <Label for="example" className="form-label required">Title</Label>
            <AvInput name="title" id="title" required />
          </AvGroup>
          
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
              <AvGroup>
                  <Label for="umDate" className="form-label">UM date</Label>
                  <AvInput name="umDate" id="umDate" value="UM date" className="m-r-15px" required />
                </AvGroup>

                <AvGroup>
                  <Label for="umWeekDateEN" className="form-label p-l-15px">UM Week Date - EN</Label>
                  <AvInput name="umWeekDateEN" id="umWeekDateEN" value="UM Week Date - EN" required />
                </AvGroup>

                <div class="w-100"></div>

                <AvGroup>
                  <Label for="startTime" className="form-label">Start time</Label>
                  <AvInput name="startTime" id="startTime" className="m-r-15px" value="11:00" required />
                </AvGroup>

                <AvGroup>
                  <Label for="endTime" className="form-label">End time</Label>
                  <AvInput name="endTime" id="endTime" value="End time" required />
                </AvGroup>

                <div class="w-100"></div>

                <AvGroup>
                  <Label for="umWeekDateCN" className="form-label">UM Week Date - CN</Label>
                  <AvInput name="umWeekDateCN" id="umWeekDateCN" value="hh:mm:ss AM/PM MM/DD/YYYY" required />
                </AvGroup>
            </div>            
          </fieldset>

          <div className="send-date-area">
            <AvField 
              type="select" 
              name="senDateTime"
              labelClass="form-label required"
              label="Send Date/Time" 
              required>

              <option>Please Select</option>
              <option>Send immediately</option>
              <option>Send at</option>
            </AvField>
            <input type="text" name="dateTime" id="dateTime" className="form-control date-time" />
            <span>(GMT +8)</span>
          </div>
          
          <Button className="btn-submit">Submit</Button>

        </AvForm>
    </div>
  </div>
  );

};

export default connect()(NewNotification);
