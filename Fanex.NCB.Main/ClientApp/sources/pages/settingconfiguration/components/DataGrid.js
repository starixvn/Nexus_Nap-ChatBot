import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';
import { Table, Row, Cell } from '../../common/fixed-table-header/index';
import Labels from '../../common/Labels';
import NoDataTitle from '../../common/no-data-title/NoDataTitle';
import TemplatePreview from '../../common/TemplatePreview/index';

import Panel, { PanelHeader } from '../../common/panel/index';

export class DataGrid extends Component {
    constructor(props) {
        super(props);
    }

    onPaging = (pageIndex, pageSize) => {
        const params = { ...this.props.gridParams };
        params['pageIndex'] = pageIndex;
        params['pageSize'] = pageSize;

        this.props.setGridParams(params);
        this.props.getGridData(params);
    }

    buildHeader = isHost => {
        return (
            <Row >
                <Cell colWidth={10} header>{Labels.NumberCol}</Cell>
                <Cell colWidth={35} header>{Labels.MsgContentsType}</Cell>
                <Cell colWidth={35} header>{Labels.Name}</Cell>
                {
                    isHost && <Cell colWidth={20} header>{Labels.Action}</Cell>
                }
            </Row>);
    };

    buildBody = isHost => {
        const { pageIndex, pageSize } = this.props.gridParams;

        this.startIndex = (pageIndex - 1) * pageSize + 1;

        if (this.props.gridData.data.length > 0) {
            return this.props.gridData.data.map((item, index) => this.renderRow(item, index, isHost));
        }

        return [<Row key={`body${1}`} ><Cell colSpan="4"><NoDataTitle /></Cell></Row>];
    }

    // handleEditSettingType = e => {
    //     const id = parseInt(e.target.dataset.templateid, 10);
    //     this.props.getTemplate(id);
    // }

    // handleDeleteSettingType = e => {
    //     const id = parseInt(e.target.dataset.templateid, 10);
    //     this.props.openDeleteTemplatePopup({ templateId: id });
    // }

    renderRow = (item, index, isHost) => (
        <Row key={item.msgContentType} >
            <Cell className="text-center">{index + this.startIndex}</Cell>
            <Cell>{item.msgContentType}</Cell>
            <Cell>{item.name}</Cell>
            {
                isHost &&
                <Cell className="text-center">
                    <>
                        <span className="icon-edit" data-templateid={item.msgContentType} onClick={this.handleEditSettingType} />
                        {
                            item.unRemovable ?
                            <span className="icon-delete icon-disabled" /> :
                            <span className="icon-delete" data-templateid={item.msgContentType} onClick={this.handleDeleteSettingType} />
                        }
                    </>
                </Cell>
            }
        </Row>
    )

    render() {
        if (!this.props.gridData || !this.props.gridData.data) {
            return null;
        }

        const { total } = this.props.gridData;
        const { pageIndex, pageSize } = this.props.gridParams;
        const { haveHostPermission } = this.props;

        const pagingOption = {
            PageIndex: pageIndex,
            PageSize: pageSize,
            TotalItem: total,
            PageList: [10, 20, 50, 100, 500]
        };

        return (
            <Table
                width={600}
                autoWidth={false}
                header={this.buildHeader(haveHostPermission)}
                body={this.buildBody(haveHostPermission)}
                isPaging
                pageOption={pagingOption}
                onPaging={this.onPaging}
                containerPadding={0}
                adjustedHeight={50}
            />
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { gridParams, gridData } = state;
    return {
        gridParams,
        gridData
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(DataGrid);