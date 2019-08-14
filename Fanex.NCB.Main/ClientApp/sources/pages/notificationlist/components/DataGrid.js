import React, { Component } from 'react';
import { Table, Row, Cell } from '../../common/fixed-table-header/index';

import { PAGING_SIZES } from '../../constants/constants';
import Labels from '../../common/Labels';
import NoDataTitle from '../../common/no-data-title/NoDataTitle';

export default class DataGrid extends Component {
    constructor(props) {
        super(props);
    }

    onPaging = (pageIndex, pageSize) => {
        this.props.onPaging(pageIndex, pageSize);
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
        const startIndex = (pageIndex - 1) * pageSize + 1;
        if (data.length > 0) {
            return data.map((item, index) => {
                return (<Row key={item.notificationId} >
                    <Cell className="col-order text-center">{index + startIndex}</Cell>
                    <Cell>{item.notificationTypeName}</Cell>
                    <Cell className="text-center">
                        {item.configSendTime.toFormat('hh:mm:ss a')}
                        <div>{item.configSendTime.toFormat('MM/dd/yyyy')}</div>
                    </Cell>

                    <Cell><strong>{item.title}</strong></Cell>
                    <Cell><span className="badge-green">{item.status}</span></Cell>
                    <Cell className="col-action"></Cell>
                </Row>);
            });
        }

        return [<Row key={`body${1}`} ><Cell colSpan="6"><NoDataTitle /></Cell></Row >];
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
            />
        );
    }
}