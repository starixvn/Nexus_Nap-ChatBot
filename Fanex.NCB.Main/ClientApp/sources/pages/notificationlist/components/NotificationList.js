import React, { Component } from 'react';
import { connect } from 'react-redux';
import {NCBBreadcrum} from '../../common/breadcrumbs';
import ErrorReport from './ErrorReport';

import "../../shared/components/font.css";
import './NotificationList.scss';

class NotificationList extends Component {
  constructor(props) {
    super(props);
    this.state = {
      isRemark: null
    };
    this.toggleHandler = this.toggleHandler.bind(this);
    this.updateRemark = this.updateRemark.bind(this);
  }

  updateRemark(event) {
    const remark = event.currentTarget.dataset.remark;

    this.setState({ isRemark: remark });
    this.props.toggleHandler();
  }

  toggleHandler() {
    this.props.toggleHandler();
  }

  render() {
    return (
      <div>
        <NCBBreadcrum chatbotName="Skype Chatbot" pageTitle="Notification List" />

        <table className="tbl w-100">
          <thead>
            <tr>
              <th className="col-order">#</th>
              <th>Sender/<br/> Notification Type</th>
              <th className="col-send-date">Send Date/Time<br/> (GMT+8)</th>
              <th>
                <div className="has-icons">Title/Content <span className="icon-expand" /></div> 
              </th>
              <th className="col-status">Status</th>
              <th className="col-action">Action</th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <td className="text-center">1</td>
              <td>
                <div>User1</div>
                <div>UM Notification</div>
              </td>
              <td className="text-center">
                <div>10:00:00 PM</div>
                <div>12/26/2018</div>
              </td>
              <td className="col-content">
                <span className="icon-down" />
                To 4 recipient(s): ACBbet, XYZbet BBIN, BBIN2 <br/>
                <strong>WEEKLY MAINTENENCE ANNOUNCEMENT - 11/26/2018</strong> <br/>
                Good Day!<br/>

                This is Shane, kindly take note of the below message [请注意以下公告]:<br/>

                >> All services subscribed with us including Sportsbook, Gaming and Back Office are not available during maintenance on going<br/>

                >> 在维护期间, 所有和我们订阅的服务, 包括体育, 娱乐厅和后台将无法使用<br/>

                Thank you for your kind attention!<br/>
              </td>
              <td><span className="badge-green">Sent</span></td>
              <td />
            </tr>
            <tr>
              <td className="text-center">2</td>
              <td>
                <div>User1</div>
                <div>UM Notification</div>
              </td>
              <td className="text-center">
                <div>10:00:00 PM</div>
                <div>12/26/2018</div>
              </td>
              <td className="col-content">
                <span className="icon-right" />
                To 4 recipient(s): ACBbet, XYZbet BBIN, BBIN2 <br/>
                <strong>WEEKLY MAINTENENCE ANNOUNCEMENT - 11/26/2018</strong>
              </td>
              <td><span className="badge-blue">Planned</span></td>
              <td className="text-center"><span className="icon-delete" /></td>
            </tr>
            <tr>
              <td className="text-center">3</td>
              <td>
                <div>User1</div>
                <div>UM Notification</div>
              </td>
              <td className="text-center">
                <div>10:00:00 PM</div>
                <div>12/26/2018</div>
              </td>
              <td className="col-content">
                <span className="icon-right" />
                To 4 recipient(s): ACBbet, XYZbet BBIN, <span className="text-red">BBIN2, Gold Deluxe</span> <br/>
                <strong>WEEKLY MAINTENENCE ANNOUNCEMENT - 11/26/2018</strong>
              </td>
              <td><span className="badge-red">Error</span></td>
              <td className="text-center"><span className="icon-view" data-remark onClick={this.updateRemark} /></td>
            </tr>
            <tr>
              <td className="text-center">4</td>
              <td>
                <div>User1</div>
                <div>UM Notification</div>
              </td>
              <td className="text-center">
                <div>10:00:00 PM</div>
                <div>12/26/2018</div>
              </td>
              <td className="col-content">
                <span className="icon-right" />
                To 4 recipient(s): ACBbet, XYZbet BBIN, <span className="text-red">BBIN2, Gold Deluxe</span> <br/>
                <strong>WEEKLY MAINTENENCE ANNOUNCEMENT - 11/26/2018</strong>
              </td>
              <td><span className="badge-orange">Resolved</span></td>
              <td className="text-center"><span className="icon-view" data-remark={false} onClick={this.updateRemark} /></td>
            </tr>
          </tbody>
        </table>

        <ErrorReport
            isModal={this.props.isModal}
            toggleHandler={this.toggleHandler}
            isRemark={this.state.isRemark}
        />
      </div>
    );
  }
}

export default connect()(NotificationList);