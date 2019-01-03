import React, { Component } from 'react';
import { connect } from 'react-redux';
import NCBBreadcrum from '../../common/breadcrumbs';
import NewNotificationTemplate from './NewNotificationTemplate';
import { Tooltip } from 'reactstrap';
import "../../../fonts/style.css";
import './NotificationTemplate.scss';

class NotificationTemplate extends Component {
  constructor(props) {
    super(props);  
    this.state = {
      tooltipOpen: false,
      modal: false
    };
    this.togglePopup = this.togglePopup.bind(this);
    this.toggle = this.toggle.bind(this);  
  }

  togglePopup() {
    this.setState({
      modal: !this.state.modal
    });
  }

  toggle() {
    this.setState({
      tooltipOpen: !this.state.tooltipOpen
    });
  }

  render() {
    return (
      <div>
        <NCBBreadcrum chatbotName="Skype Chatbot" pageTitle="Notification Template"></NCBBreadcrum>
        <form className="filter">
          <div class="form-group">
              <label htmlFor="recipientType" class="form-label">Notification Type</label>
              <select name="recipientType" required="" id="recipientType" class="notification-type form-control">
                <option>--- Please select ---</option>
                <option>UM</option>
              </select>
          </div>
          <button className="btn-submit">Submit</button>
        </form>

        <button className="btn-new" onClick={this.togglePopup}>Create New Template</button>

        <table className="tbl w-100">
          <thead>
            <tr>
              <th className="col-order">#</th>
              <th>Notification Type</th>
              <th>Notification Name</th>
              <th>
                <div className="has-icons">Title/Content <span className="icon-expand"></span></div> 
              </th>
              <th className="col-action">Action</th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <td className="text-center">1</td>
              <td>UM</td>
              <td>Weekly UM 1</td>
              <td className="col-content">
                <span className="icon-down"></span>
                <p className="bold text-uppercase">Weekly maintenance announcement - <span className="text-red">UM DATE</span></p>
                Good Day! <br/>
                This is Shane, kindly take note of the below message <br/>
              </td>
              <td className="text-center col-action">
                <span className="icon-edit" id="editTooltip1"></span>
                <Tooltip placement="top" isOpen={this.state.tooltipOpen} target="editTooltip1" toggle={this.toggle}>
                  Edit
                </Tooltip>
                <span className="icon-delete"></span>
              </td>
            </tr>
            <tr>
              <td className="text-center">1</td>
              <td>UM</td>
              <td>Weekly UM 2</td>
              <td className="col-content">
                <span className="icon-right"></span>
                <span className="bold text-uppercase">Weekly maintenance (complete) announcement - <span className="text-red">UM DATE</span></span>
              </td>
              <td className="text-center col-action">
                <span className="icon-edit"></span>
                <span className="icon-delete"></span>
              </td>
            </tr>
            <tr>
              <td className="text-center">1</td>
              <td>UM</td>
              <td>Extend UM</td>
              <td className="col-content">
                <span className="icon-right"></span>
                <span className="bold text-uppercase">Extend maintenance announcement</span>
              </td>
              <td className="text-center col-action">
                <span className="icon-edit"></span>
                <span className="icon-delete"></span>
              </td>
            </tr>
            <tr>
              <td className="text-center">1</td>
              <td>UM 3rd</td>
              <td>UM 3rd - Schedule</td>
              <td className="col-content">
                <span className="icon-right"></span>
                <span className="bold text-uppercase">UM Confirmation - <span className="text-red">UM DATE</span></span>
              </td>
              <td className="text-center col-action">
                <span className="icon-edit"></span>
                <span className="icon-delete"></span>
              </td>
            </tr>
            <tr>
              <td className="text-center">1</td>
              <td>UM 3rd</td>
              <td>UM 3rd 1</td>
              <td className="col-content">
                <span className="icon-right"></span>
                <span className="bold text-uppercase">maintenance announcement - <span className="text-red">UM DATE</span></span>
              </td>
              <td className="text-center col-action">
                <span className="icon-edit"></span>
                <span className="icon-delete"></span>
              </td>
            </tr>
            <tr>
              <td className="text-center">1</td>
              <td>UM 3rd</td>
              <td>UM 3rd 2</td>
              <td className="col-content">
                <span className="icon-right"></span>
                <span className="bold text-uppercase">Match Suspend/Delay - Match Name</span>
              </td>
              <td className="text-center col-action">
                <span className="icon-edit"></span>
                <span className="icon-delete"></span>
              </td>
            </tr>
          </tbody>
        </table>
        <NewNotificationTemplate
          isModal={this.state.modal}
          toggleHandler={this.togglePopup} />
      </div>
    );
  }
}

export default connect()(NotificationTemplate);

