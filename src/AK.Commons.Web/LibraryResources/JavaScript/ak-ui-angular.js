/*******************************************************************************************************************************
 * AK.Commons.Web.LibraryResources.JavaScript.ak-ui-angular
 * Copyright © 2014 Aashish Koirala <http://aashishkoirala.github.io>
 * 
 * This file is part of Aashish Koirala's Commons Web Library (AKCWL).
 *  
 * AKCWL is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * AKCWL is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with AKCWL.  If not, see <http://www.gnu.org/licenses/>.
 * 
 *******************************************************************************************************************************/

'use strict';

// Angular module "ak.ui" to house all directives defined within this package.
//
angular.module('ak.ui', []);

// Directive to force the value of a textbox to a defined integer range.
//
angular.module('ak.ui').directive('akUiForceInteger', function() {

    return {
        restrict: 'A',
        require: 'ngModel',

        link: function(scope, element, attributes, ngModel) {

            if (!ngModel) return;

            ngModel.$parsers.unshift(function(inputValue) {

                var value = isNaN(inputValue) ? 0 : inputValue;

                var minValue = attributes.akUiMinValue || 0;
                if (value < minValue) value = minValue;

                if (attributes.akUiMaxValue != undefined &&
                    attributes.akUiMaxValue != null &&
                    value > attributes.akUiMaxValue) {

                    value = attributes.choreMaxValue;
                }

                ngModel.$viewValue = value;
                ngModel.$render();

                return value;
            });
        }
    };
});

// Directive to prevent the click event from firing on an HTML element.
//
angular.module('ak.ui').directive('akUiNoClick', function () {

    return {
        restrict: 'A',
        link: function (scope, element) {

            element.click(function(event) {
                event.stopPropagation();
            });
        }
    };
});

// Directive to prevent a Bootstrap drop-down from getting closed when
// clicked outside.
//
angular.module('ak.ui').directive('akUiBsDropdownNoHide', function () {

    return {
        restrict: 'A',
        link: function (scope, element) {

            element.bind('hide.bs.dropdown', function () {
                return false;
            });
        }
    };
});

// Directive for textbox that auto-grows with content.
//
angular.module('ak.ui').directive('akUiElastic', ['$timeout', function($timeout) {
    return {
        restrict: 'A',
        link: function(scope, element, attributes) {
            element.on("blur keyup change", resize);
            $timeout(resize, 0);

            scope.$watch(attributes.ngBind, resize);

            function resize() {
                return element[0].style.height = "" + element[0].scrollHeight + "px";
            }
        }
    };
}]);

// Services that enables recursive directives.
//
angular.module('ak.ui').factory('directiveRecursionService', ['$compile', function ($compile) {

    // The code for this directive built upon the following:    
    // http://stackoverflow.com/questions/14430655/recursion-in-angular-directives

    return {
        compile: function (element, link) {

            if (angular.isFunction(link)) link = { post: link };

            var contents = element.contents().remove();
            var compiledContents;
            
            return {
                pre: (link && link.pre) ? link.pre : null,
                post: function(scope, e) {
                    if (!compiledContents) compiledContents = $compile(contents);

                    compiledContents(scope, function(clone) {
                        e.append(clone);
                    });

                    if (link && link.post) link.post.apply(null, arguments);
                }
            };
        }
    };
}]);