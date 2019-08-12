import React, { Component } from 'react';
import axios from 'axios';
import { NCBBreadcrum } from '../../common/breadcrumbs';
import { Form, FormGroup, Label, Button, Container, Row, Col, Input, FormFeedback } from 'reactstrap';
import ErrorReport from './ErrorReport';
import DatePicker from '../../common/DatePicker/index';
import { MultipleSelectField } from '../../common/MultipleSelectField';
import Labels from '../../common/Labels';
import TreeSelect from '../../common/TreeSelect/index';
import errorResponseHandler from '../../lib/errorResponseHandler';
import Site from '../../lib/site';

import '../../shared/components/font.css';
import './NotificationList.scss';

export default class NotificationList extends Component {
    constructor(props) {
        super(props);

        const today = new Date();

        this.state = {
            isRemark: null,
            filter: {
                notificationTypes: {
                    value: ''
                },
                fromDate: new Date().setDate(today.getDate() - 7),
                toDate: today,
                creatorId: '',
                recipients: []
            },
            notificationTypeOptions: [],
            recipientOptions: []
        };
    }
    componentDidMount() {
        const promises = [
            axios(Site.resolveClientUrl('Common/GetNotificationTypes')),
            axios(Site.resolveClientUrl('Common/GetRecipients'))
        ];

        Promise.all(promises)
            .then(([notificationTypeResponse, recipientResponse]) => {
                const notificationTypeIds = [];
                const notificationTypeOptions = notificationTypeResponse.data.map(x => {
                    notificationTypeIds.push(x.notificationTypeID);
                    return {
                        id: x.notificationTypeID,
                        name: x.notificationTypeName,
                        checked: true
                    };
                });

                this.setState(prevState => ({
                    notificationTypeOptions,
                    recipientOptions: [{
                        title: Labels.All,
                        value: Labels.All,
                        children: recipientResponse.data
                    }],
                    filter: {
                        ...prevState.filter,
                        ...{
                            notificationTypes: {
                                value: notificationTypeIds.toString()
                            },
                            recipients: [{
                                title: Labels.All,
                                value: Labels.All
                            }]
                        }
                    }
                }));
            }).catch(errorResponseHandler);
    }

    handleFilterChange = (name, value) => {
        this.setState(prevState => ({
            filter: { ...prevState.filter, ...{ [name]: value } }
        }));
    }

    handleFromDateChange = value => this.handleFilterChange('fromDate', value)

    handleToDateChange = value => this.handleFilterChange('toDate', value)

    handleNotificationTypeChange = value => this.handleFilterChange('notificationTypes', value)

    handleCreatorChange = e => this.handleFilterChange('creatorId', e.target.value)

    handleRecipientChange = (value, label, extra) => {
        debugger;
        this.handleFilterChange('recipients', value);
    }

    updateRemark = event => {
        const remark = event.currentTarget.dataset.remark;
        this.setState({
            isRemark: remark
        });
        this.props.toggleHandler();
    }

    toggleHandler = () => {
        this.props.toggleHandler();
    }

    render() {
        return (
            <div>
                <NCBBreadcrum chatbotName="Skype Chatbot" pageTitle="Notification List" />

                <Form className="form-filter" >
                    <FormGroup>
                        <Label className="form-label">Notification Type</Label>
                        <MultipleSelectField
                            width={182}
                            initialValue={this.state.notificationTypes}
                            data={this.state.notificationTypeOptions}
                            keyField="id"
                            valueField="name"
                            statusField="checked"
                            isSummited={false}
                            isAllTextShown
                            onChange={this.handleNotificationTypeChange}
                        />
                    </FormGroup>
                    <FormGroup>
                        <Label className="form-label">From</Label>
                        <DatePicker selected={this.state.filter.fromDate} onChange={this.handleFromDateChange} />
                    </FormGroup>
                    <FormGroup>
                        <Label className="form-label">To</Label>
                        <DatePicker selected={this.state.filter.toDate} onChange={this.handleToDateChange} />

                    </FormGroup>
                    <FormGroup>
                        <Label className="form-label">Sender</Label>
                        <Input type="select" name="creatorId" value={this.state.filter.creatorId} onChange={this.handleCreatorChange}>
                            <option value="" >{Labels.All}</option>
                            <option value="8" >sa</option>

                        </Input>
                    </FormGroup>
                    <FormGroup>
                        <Label className="form-label">Recipient</Label>
                        <TreeSelect value={this.state.filter.recipients} onChange={this.handleRecipientChange}
                            placeholder={Labels.PleaseSelect} maxTagCount={2} treeData={this.state.recipientOptions}
                        />
                    </FormGroup>
                    <button id="submit-button" className="btn-submit">Submit</button>
                </Form>
                <table className="tbl w-100">
                    <thead>
                        <tr>
                            <th className="col-order">#</th>
                            <th>Sender/<br /> Notification Type</th>
                            <th className="col-send-date">Send Date/Time<br /> (GMT+8)</th>
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
                                To 4 recipient(s): ACBbet, XYZbet BBIN, BBIN2 <br />
                                <strong>WEEKLY MAINTENENCE ANNOUNCEMENT - 11/26/2018</strong> <br />
                                Good Day!<br />

                                This is Shane, kindly take note of the below message [请注意以下公告]:<br />

                                >> All services subscribed with us including Sportsbook, Gaming and Back Office are not available during maintenance on going<br />

                                >> 在维护期间, 所有和我们订阅的服务, 包括体育, 娱乐厅和后台将无法使用<br />

                                Thank you for your kind attention!<br />
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
                                To 4 recipient(s): ACBbet, XYZbet BBIN, BBIN2 <br />
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
                                To 4 recipient(s): ACBbet, XYZbet BBIN, <span className="text-red">BBIN2, Gold Deluxe</span> <br />
                                <strong>WEEKLY MAINTENENCE ANNOUNCEMENT - 11/26/2018</strong>
                            </td>
                            <td><span className="badge-red">Error</span></td>
                            <td className="text-center"><span className="icon-view" data-remark onClick={this.updateRemark} /></td>
                        </tr>
                        <tr>
                            <td className="text-center">4</td>
                            <td>
                                <div>User2</div>
                                <div>UM Notification</div>
                            </td>
                            <td className="text-center">
                                <div>10:00:00 PM</div>
                                <div>12/26/2018</div>
                            </td>
                            <td className="col-content">
                                <span className="icon-right" />
                                To 4 recipient(s): ACBbet, XYZbet BBIN, <span className="text-red">BBIN2, Gold Deluxe</span> <br />
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