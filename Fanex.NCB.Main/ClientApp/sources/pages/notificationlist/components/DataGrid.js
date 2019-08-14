import React, { Component } from 'react';
import { Table, Row, Cell } from '../../common/fixed-table-header/index';

import { PAGING_SIZES, NotificationStatus } from '../../constants/constants';
import Labels from '../../common/Labels';
import NoDataTitle from '../../common/no-data-title/NoDataTitle';

export default class DataGrid extends Component {
    constructor(props) {
        super(props);
    }

    handleCancelNotification = e => {
        var id = parseInt(e.target.dataset.notificationid);
        this.props.cancelNotification(id);
    }

    onPaging = (pageIndex, pageSize) => {
        this.props.onPaging(pageIndex, pageSize);
    }

    getNotificationStatusClass = statusId => {
        switch (statusId) {
            case NotificationStatus.Planned: return "badge-blue";
            case NotificationStatus.Sent: return "badge-green  ";
            case NotificationStatus.Error: return "badge-red";
            case NotificationStatus.Sending: return "badge-outline-green";
            case NotificationStatus.Cancelled: return "badge-grey";
            case NotificationStatus.Resolved: return "badge-orange";
        }
    }

    buildHeader = () => {
        return (
            <Row style={{ height: 32 }}>
                <Cell colWidth={5} header>{Labels.NumberCol}</Cell>
                <Cell colWidth={20} header>Sender/Notification Type</Cell>
                <Cell colWidth={18} header>Send Date/Time (GMT+8)</Cell>
                <Cell colWidth={100} header><div className="has-icons">Title/Content <span className="icon-expand" /></div></Cell>
                <Cell colWidth={12} className="col-status" header>Status</Cell>
                <Cell colWidth={10} className="col-action" header>{Labels.Action}</Cell>
            </Row>);
    };

    buildBody = () => {
        const { data, pageIndex, pageSize } = this.props.tableData;
        this.startIndex = (pageIndex - 1) * pageSize + 1;
        if (data.length > 0) {
            return data.map(this.renderRow);
        }

        return [<Row key={`body${1}`} ><Cell colSpan="6"><NoDataTitle /></Cell></Row >];
    }

    renderRow = (item, index) => {
        return (<Row key={item.notificationId} >
            <Cell className="col-order text-center">{index + this.startIndex}</Cell>
            <Cell>{item.notificationTypeName}</Cell>
            <Cell className="text-center">
                {item.configSendTime.toFormat('hh:mm:ss a')}
                <div>{item.configSendTime.toFormat('MM/dd/yyyy')}</div>
            </Cell>

            <Cell><strong>{item.title} - {item.configSendTime.toFormat('MM/dd/yyyy')}</strong></Cell>
            <Cell className="text-center"><span className={this.getNotificationStatusClass(item.statusId)}>{item.status}</span></Cell>
            <Cell className="col-action text-center">{this.renderAction(item)}</Cell>
        </Row>);
    }

    renderAction = notification => {
        switch (notification.statusId) {
            case NotificationStatus.Error:
            case NotificationStatus.Resolved:
                return (<span className="icon-view" data-notificationid={notification.notificationId} onClick={this.props.showErrorReport} />);
            case NotificationStatus.Planned:
                return (<span className="icon-delete" data-notificationid={notification.notificationId} onClick={this.handleCancelNotification} />);
            case NotificationStatus.Cancelled:
                return (<a className="icon-edit" href={`EditNotification/${notification.notificationId}`} />);
            default: return '';
        }
    }

    render() {
        const pagingOption = {
            PageIndex: this.props.tableData.pageIndex,
            PageSize: this.props.tableData.pageSize,
            TotalItem: this.props.tableData.total,
            PageList: PAGING_SIZES
        };

        return (
            <Table
                width={1620}
                autoWidth={false}
                header={this.buildHeader()}
                body={this.buildBody()}
                isPaging
                pageOption={pagingOption}
                onPaging={this.onPaging}
                containerPadding={0}
                adjustedHeight={50}
            />
        );
    }
}