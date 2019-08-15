import React, { Component, Fragment } from 'react';
import { Table, Row, Cell } from '../../common/fixed-table-header/index';

import { PAGING_SIZES, NotificationStatus, DateFormat, TimeFormat } from '../../constants/constants';
import Labels from '../../common/Labels';
import NoDataTitle from '../../common/no-data-title/NoDataTitle';
import Panel, { PanelHeader } from './Panel';

export default class DataGrid extends Component {
    constructor(props) {
        super(props);

        this.state = {
            expandPanels: false
        };

        this.panels = [];
    }

    toggleExpandPanels = () => {
        this.setState(prevState => ({ expandPanels: !prevState.expandPanels }), () => {
            this.panels.filter(x => x.state.isOpen !== this.state.expandPanels).forEach(panel => {
                panel.set(this.state.expandPanels);
            });
        });
    }

    handleCancelNotification = e => {
        var id = parseInt(e.target.dataset.notificationid);
        this.props.onCancelNotification(id);
    }

    handleShowErrorReport = e => {
        var id = parseInt(e.target.dataset.notificationid);
        this.props.onShowErrorReport(id);
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
                <Cell colWidth={100} header>
                    <div className="has-icons">
                        Title/Content
                    <span className={this.state.expandPanels ? 'icon-collapse' : 'icon-expand'} onClick={this.toggleExpandPanels} />
                    </div>
                </Cell>
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

        return [<Row key={`body${1}`} ><Cell colSpan="6"><NoDataTitle /></Cell></Row>];
    }

    renderRecipient = (recipient, index) => (
        <Fragment key={index}>
            {index > 0 && ', '}
            <span className={recipient.isError === 1 ? 'text-red' : ''} > {recipient.recipientName}</span>
        </Fragment>
    )

    renderRow = (item, index) => {
        return (<Row key={item.notificationId} >
            <Cell className="col-order text-center">{index + this.startIndex}</Cell>
            <Cell>{item.notificationTypeName}</Cell>
            <Cell className="text-center">
                {item.configSendTime.toFormat(TimeFormat)}
                <div>{item.configSendTime.toFormat(DateFormat)}</div>
            </Cell>
            <Cell>
                <Panel ref={panel => this.panels[index] = panel}>
                    <PanelHeader>
                        <div>To {item.recipientSentStatusJson.length} recipient(s): {item.recipientSentStatusJson.map(this.renderRecipient)}</div>
                        <strong>{item.title} - {item.configSendTime.toFormat('MM/dd/yyyy')}</strong>
                    </PanelHeader>
                    <p className="notification-content" >{item.content}</p>
                </Panel>
            </Cell>
            <Cell className="text-center"><span className={this.getNotificationStatusClass(item.statusId)}>{item.status}</span></Cell>
            <Cell className="col-action text-center">{this.renderAction(item)}</Cell>
        </Row>);
    }

    renderAction = notification => {
        switch (notification.statusId) {
            case NotificationStatus.Error:
            case NotificationStatus.Resolved:
                return (<span className="icon-view" data-notificationid={notification.notificationId} onClick={this.handleShowErrorReport} />);
            case NotificationStatus.Planned:
                return (<span className="icon-delete" data-notificationid={notification.notificationId} onClick={this.handleCancelNotification} />);
            case NotificationStatus.Cancelled:
                return (<a className="icon-edit" href={`EditNotification/${notification.notificationId}`} />);
            default: return (<span className="icon-view" data-notificationid={notification.notificationId} onClick={this.handleShowErrorReport} />);
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