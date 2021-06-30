import React from 'react';
import { shallow } from 'enzyme';

import MultipleSelectLabel from '../MultipleSelectLabel';

describe('MultipleSelectLabel', () => {
    const defaultSelectedItem = {
        key: 'keyDefault',
        value: 'valueDefault'
    };
    const defaultProps = {
        selectedItems: [],
        maxDisplayItemCount: 2,
        texts: {
            All: 'All',
            SelectedItemCount: '# selected',
            SelectOptions: 'Select options'
        }
    };

    afterAll(() => {
        jest.resetAllMocks();
    });

    describe('selectedItemsString', () => {
        it('No item is selected - return as expected', () => {
            // Arrange
            const expected = 'Select options';
            const container = shallow(<MultipleSelectLabel {...defaultProps} />).instance();

            // Act
            const actual = container.selectedItemsString;

            // Assert
            expect(actual).toBe(expected);
        });

        it('selectedItemCount > maxDisplayItemCount - return as expected', () => {
            // Arrange
            const expected = '3 selected';
            const selectedItems = [defaultSelectedItem, defaultSelectedItem, defaultSelectedItem];
            const props = { ...defaultProps, selectedItems };
            const container = shallow(<MultipleSelectLabel {...props} />).instance();

            // Act
            const actual = container.selectedItemsString;

            // Assert
            expect(actual).toBe(expected);
        });

        it('selectedItemCount <= maxDisplayItemCount - return as expected', () => {
            // Arrange
            const expected = 'valueDefault, value1';
            const selectedItems = [defaultSelectedItem, { key: 'key1', value: 'value1' }];
            const props = { ...defaultProps, selectedItems };
            const container = shallow(<MultipleSelectLabel {...props} />).instance();

            // Act
            const actual = container.selectedItemsString;

            // Assert
            expect(actual).toBe(expected);
        });

        it('all items selected - return as expected', () => {
            // Arrange
            const expected = 'All';
            const props = {
                ...defaultProps,
                selectedItems: [defaultSelectedItem],
                dataSourceSize: 1,
                maxDisplayItemCount: 1,
                isAllTextShown: true
            };
            const container = shallow(<MultipleSelectLabel {...props} />).instance();

            // Act
            const actual = container.selectedItemsString;

            // Assert
            expect(actual).toBe(expected);
        });
    });

    describe('render', () => {
        it('should match snapshot', () => {
            // Arrange
            const container = shallow(<MultipleSelectLabel {...defaultProps} />);

            // Act
            const actual = container.html();

            // Assert
            expect(actual).toMatchSnapshot();
        });
    });
});