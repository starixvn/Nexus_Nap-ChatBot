import React, { Component } from 'react'
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions';
import Labels from '../../common/Labels';
import { Table, Row, Cell } from '../../common/fixed-table-header/index';
import NoDataTitle from '../../common/no-data-title/NoDataTitle';
import { DateTimeFormatV2 } from '../../constants/constants';
var dateFormat = require("dateformat");

export class DataGridSystemMessage extends Component {

    buildHeader = isHost => {
        return (
            <Row >
                <Cell colWidth={10} header>{Labels.MessageID}</Cell>
                <Cell colWidth={10} header>{Labels.NotificationID}</Cell>
                <Cell colWidth={15} header>{Labels.CreatedDate}</Cell>
                <Cell colWidth={30} header>{Labels.ErrorMessage}</Cell>
            </Row>);
    };

    buildBody = isHost => {
        if (this.props.systemMessage.data) {
            return this.props.systemMessage.data.map((item, index) => this.renderRow(item, index, isHost));
        }

        return [<Row key={`body${1}`} ><Cell colSpan="5"><NoDataTitle /></Cell></Row>];
    };

    renderRow = (item, index, isHost) => (
        <Row key={index} >
            <Cell className="text-center">{item.messageID}</Cell>
            <Cell className="text-center">{item.notificationID}</Cell>
            <Cell className="text-center">{dateFormat(item.createdDate, DateTimeFormatV2)}</Cell>
            <Cell>{item.checkMsg}</Cell>
        </Row>
    )

    onPaging = (pageIndex, pageSize) => {
        const params = { notificationId : this.props.notificationId };
        params['pageIndex'] = pageIndex;
        params['pageSize'] = pageSize;

        this.props.getSystemMessage(params);
        this.props.getParamsPage(params);
    }
    
    render() {
        const { haveHostPermission } = this.props;
        const pagingOption = {
            PageIndex: this.props.paramsPage.pageIndex,
            PageSize: this.props.paramsPage.pageSize,
            TotalItem: this.props.systemMessage.total ? this.props.systemMessage.total : 1,
            PageList: [100, 200, 300, 500, 1000]
        };

        return (
            <Table
                width={1100}
                autoWidth={false}
                header={this.buildHeader(haveHostPermission)}
                body={this.buildBody(haveHostPermission)}
                isPaging
                pageOption={pagingOption}
                onPaging={this.onPaging}
                containerPadding={0}
                adjustedHeight={50}
            />
        )
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    return {
        paramsPage: state.paramsPage,
        systemMessage: state.systemMessage,
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(DataGridSystemMessage)
