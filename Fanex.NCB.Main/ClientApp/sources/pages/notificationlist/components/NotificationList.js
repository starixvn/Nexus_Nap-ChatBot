import '../../shared/components/font.css';
import './NotificationList.scss';

import React, { Component } from 'react';
import axios from 'axios';
import { DateTime } from 'luxon';
import { toast } from 'react-toastify';
import { NCBBreadcrum } from '../../common/breadcrumbs/breadcrumbs';
import { Form, FormGroup, Label, Input } from 'reactstrap';
import ErrorReport from './ErrorReport';
import Labels from '../../common/Labels';
import errorResponseHandler from '../../lib/errorResponseHandler';
import Site from '../../lib/site';
import DataGrid from './DataGrid';
import SignalR from './SignalR';
import ConfirmPopup from '../../common/confirm-popup/ConfirmPopup';
import MultiselectTreeView from '../../common/react-multiselect-treeview/MultiselectTreeView';
import buildFloatData from '../../common/react-multiselect-treeview/buildData';
import { defaultPageSize, errorId } from '../../constants/constants';
import CommonFunctions from '../../common/common-functions/CommonFunctions';
import { Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';
import DateRangePicker from 'react-bootstrap-daterangepicker';
import moment from 'moment';

export default class NotificationList extends Component {
    constructor(props) {
        super(props);

        const toDate = new Date();
        const fromDate = new Date();
        fromDate.setDate(fromDate.getDate() - 1);

        this.haveEditNotificationPermission = document.getElementById('have-edit-notification-permission').checked;

        this.state = {
            isRemark: null,
            filter: {
                notificationTypeIds: '',
                fromDate: fromDate,
                toDate: toDate,
                creatorId: '',
                recipientIds: '',
                statusId: '',
                channelIds: '',
                messageId: ''
            },
            notificationTypeOptions: [],
            floatRecipients: [],
            recipientOptions: [],
            statusOptions: [],
            userOptions: [],
            channelOptions: [],
            tableData: null,
            notificationCancellation: {
                show: false
            },
            errorReport: {
                notificationId: null,
                report: null,
                show: false,
                isSubmitting: false
            },
            outOfDateRange: {
                show: false
            }
        };

        const windowUrl = window.location.search;
        const params = new URLSearchParams(windowUrl);

        this.highlightNotification = {
            notificationId: params.get('Id'),
            notificationTypeId: params.get('typeId'),
            configSendTime: params.get('sendTime'),
            creatorId: params.get('creatorId'),
            pageIndex: params.get('pageIndex')
        };
    }

    componentDidMount() {
        const isHasHighlight = this.highlightNotification.notificationId && !isNaN(this.highlightNotification.notificationId);
        axios(Site.resolveClientUrl('NotificationList/GetSettings')).then(response => {
            const data = response.data;
            if (isHasHighlight) {
                data.notificationStatuses.forEach(status => status.checked = status.id === errorId);
                data.notificationTypes.forEach(item => item.checked = parseInt(this.highlightNotification.notificationTypeId, 10) === item.id);
            }

            const notificationTypeIds = data.notificationTypes.filter(item => item.checked).map(item => item.id).join(',');
            const statusIds = data.notificationStatuses.filter(item => item.checked).map(item => item.id).join(',');

            const recipientData = buildFloatData(data.recipients, true);

            this.setState(prevState => ({
                notificationTypeOptions: data.notificationTypes,
                floatRecipients: recipientData.floatData,
                recipientOptions: data.recipients,
                statusOptions: data.notificationStatuses,
                userOptions: data.senders,
                channelOptions: data.channels,
                filter: {
                    ...prevState.filter,
                    notificationTypeIds: isHasHighlight ?
                        this.highlightNotification.notificationTypeId.toString() : notificationTypeIds,
                    statusIds,
                    recipientIds: data.recipientIds,
                    fromDate: isHasHighlight ? new Date(this.highlightNotification.configSendTime) : prevState.filter.fromDate,
                    toDate: isHasHighlight ? new Date(this.highlightNotification.configSendTime) : prevState.filter.toDate,
                    creatorId: this.highlightNotification.creatorId,
                    channelIds: data.channelIds
                }
            }), () => {
                const pageIndex = this.highlightNotification.pageIndex ? parseInt(this.highlightNotification.pageIndex, 10) : 1;
                this.getNotifications(pageIndex);
            });
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

    checkValidDateRange = (fromDate, toDate) => {
        const oneDay = 24 * 60 * 60 * 1000;
        const range = Math.round(Math.abs((toDate.getTime() - fromDate.getTime()) / oneDay));
        if (range > 31)
            return false;
        return true;
    };

    getNotifications = (pageIndex = 1, pageSize = defaultPageSize) => {
        const MaxMessageId = 4294967295;
        const filter = this.state.filter;
        let messageId = filter.messageId;
        if (filter.messageId !== ''
            && (parseInt(filter.messageId, 10) <= 0 || parseInt(filter.messageId, 10) > MaxMessageId)) {
            messageId = MaxMessageId;
        }

        if (!this.checkValidDateRange(filter.fromDate, filter.toDate)) {
            this.setState({ outOfDateRange: { show: true } });
        } else {
            CommonFunctions.blockUI();
            axios.get(Site.resolveClientUrl('NotificationList/Get'), {
                params: {
                    notificationTypeIds: filter.notificationTypeIds,
                    fromDate: filter.fromDate.toDateString(),
                    toDate: filter.toDate.toDateString(),
                    creatorId: filter.creatorId,
                    recipientIds: filter.recipientIds,
                    notificationStatusIds: filter.statusIds,
                    channelIds: filter.channelIds,
                    messageId: messageId,
                    pageIndex: pageIndex,
                    pageSize: pageSize
                }
            }).then(response => {
                CommonFunctions.unblockUI();
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

    }

    handleFilterChange = (name, value) => {
        this.setState(prevState => ({
            filter: { ...prevState.filter, ...{ [name]: value } }
        }));
    }

    handleFromDateChange = value => this.handleFilterChange('fromDate', value)

    handleToDateChange = value => this.handleFilterChange('toDate', value)

    handleCreatorChange = e => this.handleFilterChange('creatorId', e.target.value)

    handleMessageIdChange = event => {
        const regex = /^[0-9\b]+$/;
        if (event.target.value === '' || regex.test(event.target.value)) {
            this.handleFilterChange('messageId', event.target.value);
        }
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

    onUpdateNotificationStatus = (notificationIds) => {
        const gridData = this.state.tableData;

        const handledNotiIds = this.getNotificationIdsToUpdate(notificationIds, gridData.data);

        this.getUpdatedNotification(handledNotiIds, (data) => {
            if (data.length > 0) {
                let tableData = { ...this.state.tableData };
                tableData.data.map(noti => {
                    let index = data.findIndex(x => x.notificationId === noti.notificationId);

                    if (index !== -1) {
                        noti = Object.assign(noti, data[index]);
                    }
                    return noti;
                });

                tableData.data = this.compareWithCurrentFilter(tableData.data);

                this.setState({ tableData: tableData });
            }
        });
    }

    onInsertedNewNotification = (notificationIds) => {
        const gridData = this.state.tableData;
        const newNotiIds = this.getNewNotificationIds(notificationIds, gridData.data);

        this.getUpdatedNotification(newNotiIds, (data) => {
            let tableData = { ...this.state.tableData };
            data.map(noti => tableData.data.unshift(noti));
            tableData.data = this.compareWithCurrentFilter(tableData.data);

            this.setState({ tableData: tableData });
        });
    }

    compareWithCurrentFilter = (notifications) => {
        const filter = this.state.filter;
        const notificationTypeIds = filter.notificationTypeIds.split(',');
        const fromDate = new Date(filter.fromDate);
        const toDate = new Date(filter.toDate);
        const creatorId = filter.creatorId;
        const recipientNames = this.state.floatRecipients.filter(item => item.isLeaf).map(item => item.text);
        const statusIds = filter.statusIds.split(','),
            channelIds = filter.channelIds.split(','),
            messageId = filter.messageId;

        let filteredNotifications = notifications
            .filter(n => notificationTypeIds.some(t => parseInt(t, 10) === n.notificationTypeId))
            .filter(x => new Date(x.configSendTime) >= new Date(fromDate.toDateString()) &&
                new Date(new Date(x.configSendTime).toDateString()) <= new Date(toDate.toDateString()))
            .filter(i => creatorId === null || parseInt(creatorId, 10) === i.creatorId)
            .filter(t => statusIds.some(st => parseInt(st, 10) === t.statusId))
            .filter(n => n.recipientSentStatusJson.map(x => x.recipientName).every(v => recipientNames.includes(v)))
            .filter(noti => channelIds.some(x => parseInt(x, 10) === noti.channelId))
            .filter(noti => messageId === '' || parseInt(messageId, 10) === noti.messageID);

        return filteredNotifications;
    };

    getNewNotificationIds = (notificationIds, notifications) => {
        const ids = notificationIds.split(',');
        const newIds = ids.filter(id => !notifications.some(n => n.notificationId === parseInt(id, 10)));
        return newIds.join(',');
    }

    getNotificationIdsToUpdate = (notificationIds, notifications) => {
        const ids = notificationIds.split(',');
        const updatedNotificationIds = notifications.filter(noti => ids.some(id => parseInt(id) === noti.notificationId));
        const result = updatedNotificationIds.map(noti => noti.notificationId);
        return result.join(',');
    }

    getUpdatedNotification = (notificationIds, callback) => {
        axios.get(Site.resolveClientUrl('NotificationList/GetUpdatedNotification'), {
            params: {
                notificationIds
            }
        }).then(response => {
            response.data.forEach(notification => {
                notification.configSendTime = DateTime.fromISO(notification.configSendTime);
                notification.recipientSentStatusJson = JSON.parse(notification.recipientSentStatusJson);
            });
            callback(response.data);
        });
    }

    getDateRangeSelect = () => {
        return {
            'Today': [moment().toDate(), moment().toDate()],
            'Yesterday': [
                moment().subtract(1, 'days').toDate(),
                moment().subtract(1, 'days').toDate()
            ],
            'Last 2 Days': [
                moment().subtract(1, 'days').toDate(),
                moment().toDate()
            ],
            'Last 7 Days': [
                moment().subtract(6, 'days').toDate(),
                moment().toDate()
            ],
            'Last 31 Days': [
                moment().subtract(30, 'days').toDate(),
                moment().toDate()
            ],
            'This Month': [
                moment().startOf('month').toDate(),
                moment().endOf('month').toDate()
            ],
            'Last Month': [
                moment().subtract(1, 'month').startOf('month').toDate(),
                moment().subtract(1, 'month').endOf('month').toDate()
            ]
        }
    };

    handleCloseOutRangeDate = () => this.setState({ outOfDateRange: { show: false } });

    handleDateChange = (fromDate, toDate) => {
        this.setState(prevState => ({
            filter: { ...prevState.filter, fromDate: fromDate.toDate(), toDate: toDate.toDate() }
        }));
    };

    render() {
        const { filter } = this.state;
        const labelDate = `${moment(filter.fromDate).format('MM/DD/YYYY')} - ${moment(filter.toDate).format('MM/DD/YYYY')}`;

        return (
            <div>
                <NCBBreadcrum chatbotName={Labels.ChatbotSystem} pageTitle="Notification List" />
                <SignalR
                    onInsertNewNotification={this.onInsertedNewNotification}
                    onUpdateNotificationStatus={this.onUpdateNotificationStatus}
                />
                <Form className="form-filter-new" onSubmit={this.handleSearch}>
                    <div className="form-row">
                        <FormGroup className="notificationtype-group">
                            <Label className="form-label">Notification Type</Label>
                            <MultiselectTreeView
                                data={this.state.notificationTypeOptions}
                                isSelectedAll hasSearchBox
                                onChange={selectedItems => this.handleFilterChange('notificationTypeIds', selectedItems.map(item => item.id).join(','))}
                            />
                        </FormGroup>
                        <FormGroup className="date-range">
                            <Label className="form-label">Date Range</Label>
                            <DateRangePicker
                                initialSettings={{
                                    startDate: filter.fromDate,
                                    endDate: filter.toDate,
                                    ranges: this.getDateRangeSelect()
                                }}
                                onCallback={this.handleDateChange}
                            >
                                <div className="form-control inputRange">
                                    <i className="fa fa-calendar iconRange" aria-hidden="true"></i>
                                    <span className="label-date">{labelDate}</span>
                                    <span className='cr-down-date'><i className="fa fa-caret-down iconRange"></i></span>
                                </div>
                            </DateRangePicker>
                        </FormGroup>
                        <FormGroup className="sender-group">
                            <Label className="form-label">Sender</Label>
                            <Input type="select" name="creatorId"
                                className="sender"
                                value={filter.creatorId ? filter.creatorId : ''}
                                onChange={this.handleCreatorChange}>
                                <option value="" >{Labels.All}</option>
                                {this.state.userOptions.map(this.renderUserOption)}
                            </Input>
                        </FormGroup>
                        <FormGroup>
                            <Label className="form-label">Recipient</Label>
                            <MultiselectTreeView
                                data={this.state.recipientOptions}
                                isSelectedAll hasSearchBox
                                onChange={selectedItems => this.handleFilterChange('recipientIds', selectedItems.map(item => item.id).join(','))}
                            />
                        </FormGroup>
                        <FormGroup className="status-group">
                            <Label className="form-label">Status</Label>
                            <MultiselectTreeView
                                data={this.state.statusOptions}
                                isSelectedAll
                                onChange={selectedItems => this.handleFilterChange('statusIds', selectedItems.map(item => item.id).join(','))}
                            />
                        </FormGroup>
                        <FormGroup className="botservice-group">
                            <Label className="form-label">{Labels.BotService}</Label>
                            <MultiselectTreeView
                                data={this.state.channelOptions}
                                isSelectedAll
                                onChange={selectedItems => this.handleFilterChange('channelIds', selectedItems.map(item => item.id).join(','))}
                            />
                        </FormGroup>
                        <FormGroup className="message-group">
                            <Label className="form-label">MessageID</Label>
                            <Input type="text" id="messageId" name="messageId" value={filter.messageId}
                                onChange={this.handleMessageIdChange}
                            />
                        </FormGroup>
                        <FormGroup>
                            <button id="submit-button" className="btn-submit" type="submit">Submit</button>
                        </FormGroup>
                    </div>
                </Form>
                {this.state.tableData &&
                    <DataGrid tableData={this.state.tableData}
                        onPaging={this.getNotifications}
                        onCancelNotification={this.handleOpenNotificationCancellation}
                        onShowErrorReport={this.handleShowErrorReport}
                        haveEditNotificationPermission={this.haveEditNotificationPermission}
                        highlightId={this.highlightNotification.notificationId}
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
                {
                    this.state.outOfDateRange.show &&
                    <Modal isOpen backdrop="static">
                        <ModalHeader>Alert</ModalHeader>
                        <ModalBody>
                            Out of data range limit! Please select a time range within 31 days maximum!
                        </ModalBody>
                        <ModalFooter>
                            <button className="btn-submit" onClick={this.handleCloseOutRangeDate}>OK</button>
                        </ModalFooter>
                    </Modal>
                }
            </div>
        );
    }
}