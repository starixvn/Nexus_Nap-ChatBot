import '../../shared/components/font.css';
import './NotificationList.scss';

import React, { Component } from 'react';
import axios from 'axios';
import { DateTime } from 'luxon';
import { toast } from 'react-toastify';
import { NCBBreadcrum } from '../../common/breadcrumbs/breadcrumbs';
import { Form, FormGroup, Label, Input } from 'reactstrap';
import ErrorReport from './ErrorReport';
import DatePicker from '../../common/DatePicker/index';
import { MultipleSelectField } from '../../common/multiple-select-field/MultipleSelectField';
import Labels from '../../common/Labels';
import TreeSelect, { getRealValuesFromCollection } from '../../common/TreeSelect/index';
import errorResponseHandler from '../../lib/errorResponseHandler';
import Site from '../../lib/site';
import DataGrid from './DataGrid';
import SignalR from './UpdateNotifictaionStatus';
import ConfirmPopup from '../../common/confirm-popup/ConfirmPopup';

export default class NotificationList extends Component {
    constructor(props) {
        super(props);

        const toDate = new Date();
        const fromDate = new Date();
        fromDate.setDate(fromDate.getDate() - 7);

        this.haveEditNotificationPermission = document.getElementById('have-edit-notification-permission').checked;

        this.state = {
            isRemark: null,
            filter: {
                notificationTypes: {
                    value: ''
                },
                fromDate: fromDate,
                toDate: toDate,
                creatorId: '',
                recipients: [],
                statuses: ''
            },
            notificationTypeOptions: [],
            recipientOptions: [],
            statusOptions: [],
            userOptions: [],
            tableData: null,
            notificationCancellation: {
                show: false
            },
            errorReport: {
                notificationId: null,
                report: null,
                show: false,
                isSubmitting: false
            }
        };
    }
    componentDidMount() {
        const promises = [
            axios(Site.resolveClientUrl('Common/GetNotificationTypes')),
            axios(Site.resolveClientUrl('Common/GetRecipients')),
            axios(Site.resolveClientUrl('Common/GetNotificationStatuses')),
            axios(Site.resolveClientUrl('Common/GetUsers'))

        ];

        Promise.all(promises)
            .then(([notificationTypeResponse, recipientResponse, statusResponse, userResponse]) => {
                const notificationTypeIds = [];
                const notificationTypeOptions = notificationTypeResponse.data.map(x => {
                    notificationTypeIds.push(x.notificationTypeID);
                    return {
                        id: x.notificationTypeID,
                        name: x.notificationTypeName,
                        checked: true
                    };
                });

                statusResponse.data.forEach(status => status.checked = true);

                this.setState(prevState => ({
                    notificationTypeOptions,
                    recipientOptions: recipientResponse.data,
                    statusOptions: statusResponse.data,
                    userOptions: userResponse.data,
                    filter: {
                        ...prevState.filter,
                        ...{
                            notificationTypes: {
                                value: notificationTypeIds.toString()
                            },
                            statuses: {
                                value: statusResponse.data.map(x => x.id).toString()
                            },
                            recipients: {
                                value: recipientResponse.data.map(r => ({ value: r.value, label: r.title })),
                                allCheckedNodes: recipientResponse.data
                            }
                        }
                    }
                }), this.getNotifications);
            }).catch(errorResponseHandler);
    }

    handleOpenNotificationCancellation = notificationId => this.setState({ notificationCancellation: { show: true, notificationId: notificationId } });

    handleCloseNotificationCancellation = () => this.setState({ notificationCancellation: { show: false } });

    handleCancelNotification = () => {
        const notificationId = this.state.notificationCancellation.notificationId;
        this.handleCloseNotificationCancellation();

        axios({
            url: Site.resolveClientUrl('NotificationList/Cancel'),
            method: 'post',
            data: `notificationId=${notificationId}`,
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': Site.getXsrfToken()
            }
        }).then(response => {
            const data = response.data;
            if (data.isSuccessful) {
                this.getNotifications(this.state.tableData.pageIndex, this.state.tableData.pageSize);
                toast.success(Labels.CancelNotificationSuccessfully);
            } else {
                toast.error(data.message);
            }
        }).catch(errorResponseHandler);
    }

    getNotifications = (pageIndex = 1, pageSize = 20) => {
        const filter = this.state.filter;
        const recipientIds = [];

        console.log(filter.recipients.allCheckedNodes);
        getRealValuesFromCollection(recipientIds, filter.recipients.allCheckedNodes);
        axios.get(Site.resolveClientUrl('NotificationList/Get'), {
            params: {
                notificationTypeIds: filter.notificationTypes.value,
                fromDate: filter.fromDate.toDateString(),
                toDate: filter.toDate.toDateString(),
                creatorId: filter.creatorId,
                recipientIds: recipientIds.toString(),
                notificationStatusIds: filter.statuses.value,
                pageIndex: pageIndex,
                pageSize: pageSize
            }
        }).then(response => {
            response.data.data.forEach(notification => {
                notification.configSendTime = DateTime.fromISO(notification.configSendTime);
                notification.recipientSentStatusJson = JSON.parse(notification.recipientSentStatusJson);
            });
            this.setState({
                tableData: {
                    pageIndex,
                    pageSize,
                    ...response.data
                }
            });
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

    handleStatusChange = value => this.handleFilterChange('statuses', value)

    handleRecipientChange = (value, label, extra) => {
        this.handleFilterChange('recipients', { value, allCheckedNodes: extra.allCheckedNodes });
    }
    renderStatus = status => <option key={status.id} value={status.id}>{status.name}</option>

    handleSearch = event => {
        event.preventDefault();
        this.getNotifications();
    }

    handleResolveNotification = resolvedRemark => {
        const notificationId = this.state.errorReport.notificationId;

        axios({
            url: Site.resolveClientUrl('NotificationList/Resolve'),
            method: 'post',
            data: {
                notificationId: notificationId,
                resolvedRemark: resolvedRemark
            },
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': Site.getXsrfToken()
            }
        }).then(response => {
            const data = response.data;

            if (data.isSuccessful) {
                this.handleCloseErrorReport();
                this.getNotifications(this.state.tableData.pageIndex, this.state.tableData.pageSize);
                toast.success(Labels.SubmitSuccessfully);
            } else {
                toast.error(data.message);
            }
        }).catch(errorResponseHandler);
    }

    handleShowErrorReport = notificationId => {
        axios.get(Site.resolveClientUrl('NotificationList/GetNotificationError'), {
            params: {
                notificationId
            }
        }).then(response => {
            const data = response.data;
            data.configSendTime = DateTime.fromISO(data.configSendTime);

            this.setState({
                errorReport: {
                    show: true,
                    notificationId: notificationId,
                    report: data
                }
            });
        }).catch(errorResponseHandler);
    }

    handleCloseErrorReport = () => this.setState({ errorReport: { show: false } });

    renderUserOption = user => <option key={user.id} value={user.id}>{user.name}</option>

    render() {
        return (
            <div>
                <NCBBreadcrum chatbotName="Skype Chatbot" pageTitle="Notification List" />
                <SignalR />
                <Form className="form-filter" onSubmit={this.handleSearch} >
                    <FormGroup>
                        <Label className="form-label">Notification Type</Label>
                        <MultipleSelectField
                            width={182}
                            initialValue={this.state.filter.notificationTypes}
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
                            {this.state.userOptions.map(this.renderUserOption)}
                        </Input>
                    </FormGroup>

                    <FormGroup>
                        <Label className="form-label">Recipient</Label>
                        <TreeSelect value={this.state.filter.recipients.value} onChange={this.handleRecipientChange}
                            placeholder={Labels.PleaseSelect} maxTagCount={2} treeData={this.state.recipientOptions}
                        />
                    </FormGroup>
                    <FormGroup>
                        <Label className="form-label">Status</Label>
                        <MultipleSelectField
                            width={182}
                            initialValue={this.state.filter.statusIds}
                            data={this.state.statusOptions}
                            keyField="id"
                            valueField="name"
                            statusField="checked"
                            isSummited={false}
                            isAllTextShown
                            onChange={this.handleStatusChange}
                        />
                    </FormGroup>
                    <button id="submit-button" className="btn-submit" type="submit">Submit</button>
                </Form>
                {this.state.tableData &&
                    <DataGrid tableData={this.state.tableData}
                        onPaging={this.getNotifications}
                        onCancelNotification={this.handleOpenNotificationCancellation}
                        onShowErrorReport={this.handleShowErrorReport}
                        haveEditNotificationPermission={this.haveEditNotificationPermission}
                    />
                }
                <ConfirmPopup
                    isOpen={this.state.notificationCancellation.show}
                    onToggle={this.handleCloseNotificationCancellation}
                    onConfirm={this.handleCancelNotification}
                    content="Cancelled notification will not be sent. Are you sure to proceed?"
                />
                {this.state.errorReport.show &&
                    <ErrorReport
                        isOpen
                        toggle={this.handleCloseErrorReport}
                        report={this.state.errorReport.report}
                        onResolveNotification={this.handleResolveNotification}
                        haveHostPermission={this.haveEditNotificationPermission}
                    />
                }

            </div>
        );
    }
}